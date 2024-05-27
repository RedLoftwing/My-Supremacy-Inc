using UnityEngine;

namespace Shaders
{
    public class TestComputeShader : MonoBehaviour
    {
        public ComputeShader computeShader;
        private ComputeBuffer _computeBuffer;
        private int _kernelHandle;
        
        struct MyLocalVertices
        {
            public uint VertexIndex;
            public Vector3 VertexVector;
        }
        
        // Start is called before the first frame update
        void Start()
        {
            int numElements = 64;
            
            MyLocalVertices[] data = new MyLocalVertices[numElements];
            for (int i = 0; i < numElements; i++)
            {
                data[i] = new MyLocalVertices
                {
                    VertexIndex = (uint)i,
                    VertexVector = new Vector3(i, i, i) // Example initialization
                };
            }

            // Create the compute buffer and fill it with data
            _computeBuffer = new ComputeBuffer(numElements, sizeof(uint) + sizeof(float) * 3);
            _computeBuffer.SetData(data);

            // Get the kernel handle from the compute shader
            _kernelHandle = computeShader.FindKernel("CSMain");

            // Set the compute buffer for the compute shader
            computeShader.SetBuffer(_kernelHandle, "localVerticesBuffer", _computeBuffer);

            // Dispatch the compute shader
            computeShader.Dispatch(_kernelHandle, 1, 1, 1);

            // Retrieve the data from the compute buffer
            _computeBuffer.GetData(data);

            // Print the modified data
            for (int i = 0; i < numElements; i++)
            {
                Debug.Log($"Index {data[i].VertexIndex}: Vector {data[i].VertexVector}");
            }

            // Release the compute buffer
            _computeBuffer.Release();
        }
    }
}