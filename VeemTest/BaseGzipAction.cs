using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VeemTest
{
    /// <summary>
    /// supertype of archived actions
    /// </summary>
    public abstract  class BaseAction : IArchivActionable
    {
        protected bool _cancelled = false;
        protected bool _success = false;
        protected string sourceFile, destinationFile;
        protected static int _threads = Environment.ProcessorCount;

        protected int blockSize = 1000000;
        protected BlockingCollection<Block> _readCollection = new BlockingCollection<Block>();
        protected BlockingCollection<Block> _writeCollection = new BlockingCollection<Block>();

        protected int _count = 1;
        protected int _lastIndex;

        

        protected int GetNextIndex()
        {
            Interlocked.Increment(ref _count);
            return _count;
        }

        protected abstract CompressionMode GetGzipReadingType();

        public BaseAction(string input, string output)
        {
            sourceFile = input;
            destinationFile = output;
        }

        public virtual void Start()
        {

            CreateFile(destinationFile + GetExtention());

            Thread _reader = new Thread(new ThreadStart(Read));
            _reader.Start();

            for (int i = 0; i < _threads; i++)
            {
                Thread _compress = new Thread(new ThreadStart(Action));
                _compress.Start();
            }

            Thread writer = new Thread(new ThreadStart(Write));
            writer.Start();
        }

        private  void Action()
        {
            try
            {
                int blockId = 0;
                while (!_cancelled && (!_readCollection.IsCompleted || _readCollection.Count > 0))
                {
                    Block block = _readCollection.Take();

                    if (block == null)
                        return;

                    Block gzipBlock = new Block()
                    {
                        ID = block.ID,
                        Buffer = GetGzipArray(block)
                    };
                    blockId = block.ID;

                    bool addRes = false;
                    while (!addRes)
                    {
                        if (_count == gzipBlock.ID && _writeCollection.Count < 10)
                        {
                            addRes = _writeCollection.TryAdd(gzipBlock, 100);
                            GetNextIndex();
                        }
                    }
                }
                if (_lastIndex == blockId)
                {
                    _writeCollection.CompleteAdding();
                    _readCollection.CompleteAdding();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error description: {ex.Message}");
                _cancelled = true;
            }
        }

        protected abstract byte[] GetGzipArray(Block block);

        public void Stop()
        {
            _cancelled = true;
        }

        protected abstract string GetExtention();

        /// <summary>
        /// Creating new file with output name
        /// </summary>
        /// <param name="destinationFile"></param>
        protected void CreateFile(string destinationFile)
        {
            using (FileStream fs = File.Create(destinationFile))
            { }
        }

        protected virtual void Read()
        {
            try
            {
                //Curent position in bytestream in input file
                long pos = 0;

                //Bytestram lengh of input file
                long fileLengh = GetFileLength(sourceFile);

                //lenght of byte for parts
                int bytesRead;

                //Byte array for ziping
                byte[] slice;

                //identifier for priority to write
                int id = 0;

                //while file lengh grater then current position and not set cancell
                while (pos < fileLengh && !_cancelled)
                {
                    bytesRead = GetReadedLenght(fileLengh, pos);

                    ////Calculating slice size
                    //if (fileLengh - pos <= blockSize)
                    //    bytesRead = (int)(fileLengh - pos);

                    //else
                    //    bytesRead = blockSize;

                    slice = GetSlice(sourceFile, pos, bytesRead);

                    //Set current position after slice
                    pos += bytesRead;

                    //increment id
                    id++;

                    Block readBlock = new Block
                    {
                        ID = id,
                        Buffer = slice
                    };

                    // not gona add another block before first 10 for saveing some RAM
                    //I gues the some better solution for that
                    while (true && !_cancelled)
                    {
                        if (_readCollection.Count() < 10)
                        {
                            _readCollection.Add(readBlock);
                            break;
                        }
                    }

                }
                //Set last id of block for indentify end for ziping
                _lastIndex = id;
               // _readCollection.CompleteAdding();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _cancelled = true;
            }
        }

        protected abstract int GetReadedLenght(long fileLengh, long pos);

        /// <summary>
        /// open input file and get byte array
        /// </summary>
        /// <param name="sourceFile">path to input file</param>
        /// <param name="pos">curent position in byte stream</param>
        /// <param name="bytesRead">size of slice array</param>
        /// <returns>byte arr of input file by giving size</returns>
        protected abstract byte[] GetSlice(string sourceFile, long pos, int bytesRead);



        /// <summary>
        /// open file and get stream lengh
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <returns></returns>
        protected long GetFileLength(string sourceFile)
        {
            long res = 0;
            using (FileStream _fileToBeCompressed = new FileStream(sourceFile, FileMode.Open))
            {
                res = _fileToBeCompressed.Length;
            }
            return res;
        }

        /// <summary>
        /// Open output file and write bytearray to it
        /// </summary>
        /// <param name="path"></param>
        /// <param name="bytes"></param>
        protected virtual void AppendAllBytes(string path, byte[] bytes)
        {
            using (var stream = new FileStream(path, FileMode.Append))
            {
                stream.Write(bytes, 0, bytes.Length);
            }
        }

        protected void Write()
        {
            try
            {
                while (true && !_cancelled && !_writeCollection.IsCompleted)
                {
                    Block _block = _writeCollection.Take();
                    AppendAllBytes(destinationFile + GetExtention(), _block.Buffer);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _cancelled = true;
            }

        }



        /// <summary>
        /// return seccess flag
        /// </summary>
        /// <returns></returns>
        public int Result()
        {
            if (!_cancelled && _success)
                return 0;
            return 1;
        }
    }
}
