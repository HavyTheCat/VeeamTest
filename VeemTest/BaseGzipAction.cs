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
    public abstract class BaseGzipAction : BaseMutltyThreadAction, IArchivActionable
    {
        public BaseGzipAction(string input, string output)
        {
            sourceFile = input;
            destinationFile = output;
        }

        protected int blockSize = 1000000;
        protected string sourceFile, destinationFile;
        private static int _threads = Environment.ProcessorCount;


        private BlockingCollection<Block> _readCollection = new BlockingCollection<Block>();
        private BlockingCollection<Block> _writeCollection = new BlockingCollection<Block>();

        private int _count = 1;
        private int _lastIndex;

        #region Implements of BaseMutltyTaskAction

        /// <summary>
        /// Start action
        /// </summary>
        public override void Start()
        {
            //Create new file by provided path
            CreateFile(destinationFile + GetExtention());

            //Create and start thread for reading input file
            Thread _reader = new Thread(new ThreadStart(Read));
            _reader.Start();

            // for each avalible on current machine process create action thread
            for (int i = 0; i < _threads; i++)
            {
                Thread _compress = new Thread(new ThreadStart(Action));
                _compress.Start();
            }
            //Create and start writing outputfile
            Thread writer = new Thread(new ThreadStart(Write));
            writer.Start();
        }

        /// <summary>
        /// stop action
        /// </summary>
        public override void Stop()
        {
            _cancelled = true;
        }

        /// <summary>
        /// return seccess flag
        /// </summary>
        /// <returns></returns>
        public override int Result() => !_cancelled && _success ? 0 : 1;

        /// <summary>
        /// Gzip action
        /// </summary>
        protected override sealed void Action()
        {
            try
            {
                int blockId = 0;
                while (!_cancelled && (!_readCollection.IsCompleted || _readCollection.Count > 0))
                {
                    Block block = new Block();

                    if (!_readCollection.TryTake(out block))
                        continue;

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
#if DEBUG
                            Console.WriteLine($"Gzip block with id :{gzipBlock.ID}");
#endif
                            if (_lastIndex == blockId)
                            {
                                _writeCollection.CompleteAdding();
                                _readCollection.CompleteAdding();
                            }
                            else
                                GetNextIndex();
                        }
                        else
                            Thread.Sleep(100);
                    }

                }


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error description: {ex.Message}");
                _cancelled = true;
            }
        }

        #endregion

        #region Abstract methods

        /// <summary>
        /// Get processed byte array
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        protected abstract byte[] GetGzipArray(Block block);

        /// <summary>
        /// Get output file extention
        /// </summary>
        /// <returns></returns>
        protected abstract string GetExtention();

        /// <summary>
        /// Get processed byte array length
        /// </summary>
        /// <param name="fileLengh"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        protected abstract int GetReadedlength(long fileLengh, long pos);

        /// <summary>
        /// Get Gzip reader reading type
        /// </summary>
        /// <returns></returns>
        protected abstract CompressionMode GetGzipReadingType();

        /// <summary>
        /// open input file and get byte array
        /// </summary>
        /// <param name="sourceFile">path to input file</param>
        /// <param name="pos">curent position in byte stream</param>
        /// <param name="bytesRead">size of slice array</param>
        /// <returns>byte arr of input file by giving size</returns>
        protected abstract byte[] GetSlice(string sourceFile, long pos, int bytesRead);

        #endregion

        #region virtual methods

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

        #endregion

        #region private methods

        /// <summary>
        /// Set index of next writing block to write collection
        /// </summary>
        /// <returns></returns>
        private int GetNextIndex()
        {
            Interlocked.Increment(ref _count);
            return _count;
        }

        /// <summary>
        /// Creating new file with output name
        /// </summary>
        /// <param name="destinationFile"></param>
        private void CreateFile(string destinationFile)
        {
            using (FileStream fs = File.Create(destinationFile))
            { }
        }

        /// <summary>
        /// read input file
        /// </summary>
        private void Read()
        {
            try
            {
                //Curent position in bytestream in input file
                long pos = 0;

                //Bytestram lengh of input file
                long fileLengh = GetFileLength(sourceFile);

                //length of byte for parts
                int bytesRead;

                //Byte array for Gziping
                byte[] slice;

                //identifier of block
                int id = 0;

                //while file length grater then current position and not set cancell
                while (pos < fileLengh && !_cancelled)
                {
                    bytesRead = GetReadedlength(fileLengh, pos);

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

                    // not gona add another block until the first ten are read.
                    //I gues the some better solution for tha saving ram 
                    while (true && !_cancelled)
                    {
                        if (_readCollection.Count() < 10)
                        {
                            _readCollection.Add(readBlock);
                            break;
                        }
                        else
                            Thread.Sleep(100);
                    }

                }
                //Set last id of block for indentify end of ziping
                _lastIndex = id;
                _readCollection.CompleteAdding();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _cancelled = true;
            }
        }

        /// <summary>
        /// open file and get stream lengh
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <returns></returns>
        private long GetFileLength(string sourceFile)
        {
            long res = 0;
            using (FileStream _fileToBeCompressed = new FileStream(sourceFile, FileMode.Open))
            {
                res = _fileToBeCompressed.Length;
            }
            return res;
        }

        /// <summary>
        /// Write method  
        /// </summary>
        private void Write()
        {
            try
            {
                while (true && !_cancelled && !_writeCollection.IsCompleted)
                {
                    Block _block = _writeCollection.Take();
                    AppendAllBytes(destinationFile + GetExtention(), _block.Buffer);
                }
                _success = true;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _cancelled = true;
            }
            finally
            {
                Console.WriteLine(Result());
            }

        }
        #endregion
    }
}
