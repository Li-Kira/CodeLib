using BERTTokenizers;
using Microsoft.ML.Data;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System;

namespace BertForCSharp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var sentence = "I'm sorry for what i said";
            //var sentence = "{\"question\": \"Where is Bob Dylan From?\", \"context\": \"Bob Dylan is from Duluth, Minnesota and is an American singer-songwriter\"}";
            Console.WriteLine(sentence);

            var tokenizer = new BertUncasedBaseTokenizer();
            var tokens = tokenizer.Tokenize(sentence);
            //Console.WriteLine(String.Join(", ", tokens));
            var encoded = tokenizer.Encode(tokens.Count, sentence);

            var bertInput = new BertInput()
            {
                InputIds = encoded.Select(t => t.InputIds).ToArray(),
                AttentionMask = encoded.Select(t => t.AttentionMask).ToArray(),
                TypeIds = encoded.Select(t => t.TokenTypeIds).ToArray(),
            };


            var modelPath = @"F:\AI\Model\Bert\bert_goemotion_6500.onnx";

            using var runOptions = new RunOptions();
            using var session = new InferenceSession(modelPath);

            // Create input tensors over the input data.
            using var inputIdsOrtValue = OrtValue.CreateTensorValueFromMemory(bertInput.InputIds,
                  new long[] { 1, bertInput.InputIds.Length });

            using var attMaskOrtValue = OrtValue.CreateTensorValueFromMemory(bertInput.AttentionMask,
                  new long[] { 1, bertInput.AttentionMask.Length });

            using var typeIdsOrtValue = OrtValue.CreateTensorValueFromMemory(bertInput.TypeIds,
                  new long[] { 1, bertInput.TypeIds.Length });

            var inputs = new Dictionary<string, OrtValue>
            {
                { "input_ids", inputIdsOrtValue },
                { "attention_mask", attMaskOrtValue },
                { "token_type_ids", typeIdsOrtValue }
            };

            // Run session and send the input data in to get inference output. 

            //error here
            using var output = session.Run(runOptions, inputs, session.OutputNames);
            var predictedClassIndex = GetMaxValueIndex(output[0].GetTensorDataAsSpan<float>());

            var emotions = new List<string>
            {
                "admiration", "amusement", "anger", "annoyance", "approval", "caring", "confusion",
                "curiosity", "desire", "disappointment", "disapproval", "disgust", "embarrassment",
                "excitement", "fear", "gratitude", "grief", "joy", "love", "nervousness", "optimism",
                "pride", "realization", "relief", "remorse", "sadness", "surprise", "neutral"
            };

            var predictedLabel = emotions[predictedClassIndex];

            Console.WriteLine($"Predicted Emotion: {predictedLabel}");

        }

        public struct BertInput
        {
            public long[] InputIds { get; set; }
            public long[] AttentionMask { get; set; }
            public long[] TypeIds { get; set; }
        }

        static int GetMaxValueIndex(ReadOnlySpan<float> span)
        {
            float maxVal = span[0];
            int maxIndex = 0;
            for (int i = 1; i < span.Length; i++)
            {
                var v = span[i];
                if (v > maxVal)
                {
                    maxVal = v;
                    maxIndex = i;
                }
            }
            return maxIndex;
        }


    }
}