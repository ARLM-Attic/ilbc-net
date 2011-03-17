using System;
using System.IO;
using iLBC;

namespace iLBCTest {
    class Program {
        static void Main(string[] args) {
            if ((args.Length == 4)) {
                int mode = 0;

                const int BLOCKL_MAX = 240;
                const int BLOCKL_20MS = 160;
                const int BLOCKL_30MS = 240;
                const int NO_OF_WORDS_20MS = 19;
                const int NO_OF_WORDS_30MS = 25;
   
                if (args[0] == "20") mode = 20;
                else if (args[0] == "30") mode = 30;

                if ((mode > 0) && File.Exists(args[1])) {
                    ilbc_encoder encoder = new ilbc_encoder(mode);
                    ilbc_decoder decoder = new ilbc_decoder(mode, 1);
                    short[] data = new short[(mode == 20) ? BLOCKL_20MS : BLOCKL_30MS];
                    short[] encodedData = new short[(mode == 20) ? NO_OF_WORDS_20MS : NO_OF_WORDS_30MS];
                    short[] decodedData = new short[BLOCKL_MAX];

                    using (BinaryReader reader = new BinaryReader(new FileStream(args[1], FileMode.Open, FileAccess.Read)))
                    using (BinaryWriter encodedWriter = new BinaryWriter(new FileStream(args[2], FileMode.Create , FileAccess.Write)),
                            decodedWriter = new BinaryWriter(new FileStream(args[3], FileMode.Create, FileAccess.Write))) {
                        bool done = false;
                        int count = 0;
                        System.Diagnostics.Stopwatch stop = new System.Diagnostics.Stopwatch();
                        stop.Start();

                        while (!done) {
                            try {
                                for (int i = 0;i < data.Length;i++) {
                                        data[i] = reader.ReadInt16();
                                }
                            } catch (EndOfStreamException) {
                                done = true;
                                break;
                            }

                            short encoded = encoder.encode(encodedData, data);


                            System.Console.Write("Encoded {0} frames \r", ++count);

                            for (short i = 0;i < (encoded / 2);i++) {
                                encodedWriter.Write(encodedData[i]);
                            }

                            short decoded = decoder.decode(decodedData, encodedData, 1);

                            for (short i = 0;i < decoded;i++) {
                                decodedWriter.Write(decodedData[i]);
                            }
                        }

                        stop.Stop();
                        System.Console.WriteLine();
                        System.Console.WriteLine("{0}", stop.Elapsed);
                    }
                    return;
                }
            }

            Console.WriteLine("\tUsage:");
            Console.WriteLine("\tiLBCTest <20|30> input encoded decoded");
        }
    }
}
