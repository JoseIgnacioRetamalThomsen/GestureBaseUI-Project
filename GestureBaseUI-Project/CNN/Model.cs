using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using TensorFlow;

namespace GestureBaseUI_Project
{
    class Model
    {
        const string INPUT_NAME = "conv2d_1_input";
        const String OUTPUT_NAME = "dense_2/Softmax";


        private TFSession _session = null;

        public Model(String modelPath)
        {
            LoadModel(modelPath);
        }

        private void LoadModel(string modelPath)
        {
            // create graph
            var graph = new TFGraph();
               
                //Import the graph from file 
                graph.Import(File.ReadAllBytes(modelPath));
                _session = new TFSession(graph);
            
        }


        /// <summary>
        /// Make a prediction for a input image
        /// </summary>
        /// <param name="image">the input image</param>
        /// <returns></returns>
        public int Predict(float[,] im)
        {
            float[] image = TransformArray(im);
            //create a runner with the session
            var runner = _session.GetRunner();

            // create tensor using image 
            var tensor = TFTensor.FromBuffer(new TFShape(1, 30, 30, 1), image, 0, image.Length);

            // set inputput layer
            runner.AddInput(_session.Graph[INPUT_NAME][0], tensor);

            // set output layer
            runner.Fetch(_session.Graph[OUTPUT_NAME][0]);

            // run the model
            var output = runner.Run();

            // Get output tensor
            TFTensor result = output[0];

            // get result from tensor
            var resultArray = result.GetValue() as float[,];

            //return 
            return ExtractPrediction(resultArray);
        }

        private float[] TransformArray(float[,] image)
        {
            float[] imageLast = new float[900];
            int ii = 0;
            for (int i = 0; i < 30; i++)
            {
                for (int j = 0; j < 30; j++)
                {
                    imageLast[ii++] = image[i, j];
                }
            }
            return imageLast;
        }

        private int ExtractPrediction(float[,] resultArray)
        {
            float[] prediction1d = new float[13];
            int i1 = 0;
            foreach (float f in resultArray)
            {
                prediction1d[i1++] = f;
            }
            float max = prediction1d.Max();
            return Array.IndexOf(prediction1d, max);
        }

        public void CreateTenso()
        {
           
            // Some input matrices


            var inp = new float[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 96, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 27, 35, 55, 82, 115, 135, 140, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 63, 77, 91, 106, 122, 136, 138, 127, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 112, 103, 95, 107, 121, 131, 131, 111, 90, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 111, 112, 101, 91, 102, 121, 126, 126, 108, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 101, 107, 96, 88, 90, 108, 125, 130, 138, 173, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 102, 98, 88, 91, 96, 101, 120, 138, 146, 185, 243, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 88, 96, 93, 91, 92, 98, 111, 135, 153, 172, 221, 242, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 90, 91, 93, 100, 107, 125, 153, 185, 210, 226, 237, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 126, 131, 140, 163, 188, 206, 216, 223, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 180, 188, 196, 202, 208, 216, 246, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 203, 203, 193, 200, 210, 233, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 227, 232, 222, 216, 231, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 233, 247, 242, 236, 246, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 151, 155, 0, 183, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            Debug.WriteLine("TEST");
            for (int i = 0; i < inp.Length; i++)
            {
                if (inp[i] == 0)
                    inp[i] = 255;

                // inp[i] = inp[i] / 255;

                //Debug.WriteLine(inp[i]);
            }
           // Debug.WriteLine("len : " + inp.Length);

            var tensor = TFTensor.FromBuffer(new TFShape(1, 30, 30, 1), inp, 0, inp.Length);

           // Debug.WriteLine("input: " + tensor);

            using (var graph = new TFGraph())
            {
                // Load the model
                //  graph.Import(File.ReadAllBytes(@"C:\Users\Public\TestFolder\my_model.pb"));
                string model_file = Path.Combine(Environment.CurrentDirectory, @"model\gesture_model.pb");

                Debug.WriteLine("file path " + model_file);
                graph.Import(File.ReadAllBytes(model_file));


                using (var session = new TFSession(graph))
                {
                    var runner = session.GetRunner();
                    //Debug.WriteLine(graph);
                    runner.AddInput(graph[INPUT_NAME][0], tensor);
                    runner.Fetch(graph[OUTPUT_NAME][0]);
                    Debug.WriteLine(System.DateTime.Now);

                    var output = runner.Run();
                    TFTensor result = output[0];
                    var re = result.GetValue() as float[,];
                    foreach (float f in re)
                    {
                        Debug.WriteLine(f);
                    }
                    Debug.WriteLine("o: " + re[0, 1]);
                    Debug.WriteLine(result);
                    Debug.WriteLine(System.DateTime.Now);
                }
            }

        }
    }
}

