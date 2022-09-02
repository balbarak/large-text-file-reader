using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ltfr.App
{
    public class FileReaderManager
    {
        public FileInfo File { get; private set; }

        public event EventHandler<ReadOnlySequence<byte>> OnBlockRead;

        public async Task ReadFile(CancellationToken token = default)
        {
            using (FileStream fs = new FileStream(File.FullName, FileMode.Open, FileAccess.Read))
            {
                var pipeReader = PipeReader.Create(fs);

                await ReadPipeAsync(pipeReader, token);
            }
        }

        public void SetFile(string file)
        {
            File = new FileInfo(file);
        }

        private async Task ReadPipeAsync(PipeReader reader, CancellationToken token)
        {
            bool isCancelled = false;

            while (true)
            {
                ReadResult result = await reader.ReadAsync();
                ReadOnlySequence<byte> buffer = result.Buffer;

                while (TryReadLine(ref buffer, out ReadOnlySequence<byte> line))
                {
                    await Task.Delay(10);

                    if (token.IsCancellationRequested)
                    {
                        isCancelled = true;
                        break;
                    }

                    OnBlockRead?.Invoke(this, line);
                }

                if (isCancelled)
                    break;

                reader.AdvanceTo(buffer.Start, buffer.End);

                if (result.IsCompleted)
                {
                    break;
                }
            }

            await reader.CompleteAsync();
        }
        private bool TryReadLine(ref ReadOnlySequence<byte> buffer, out ReadOnlySequence<byte> line)
        {
            SequencePosition? position = buffer.PositionOf((byte)'\n');

            if (position == null)
            {
                line = default;
                return false;
            }

            line = buffer.Slice(0, position.Value);
            buffer = buffer.Slice(buffer.GetPosition(1, position.Value));
            return true;
        }
    }
}
