﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using VertexCover.Src.GraphViz;

namespace VertexCover
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MatrixBuilder matrixBuilder = new MatrixBuilder();
        private int imagesGenerated = 0;
        private bool[,] matrix = new bool[0, 0];

        public MainWindow()
        {
            InitializeComponent();

            GenerateDefaultGraph();
        }

        private void GenerateGraphButton_Click(object sender, RoutedEventArgs e)
        {
            GenerateMatrixWindow generateWindow = new GenerateMatrixWindow(matrixBuilder);
            generateWindow.ShowDialog();

            if (!generateWindow.Completed)
                return;

            matrix = generateWindow.Matrix;
            DrawGraph(matrix);
        }

        private void GenerateVertexCoverButton_Click(object sender, RoutedEventArgs e)
        {
            GenerateVertexCoverWindow generateVertexCoverWindow = new GenerateVertexCoverWindow(matrix);
            generateVertexCoverWindow.ShowDialog();

            if (!generateVertexCoverWindow.Completed)
                return;

            bool[] vertexCover = generateVertexCoverWindow.VertexCover;

            if(vertexCover == null)
            {
                VertexCoverOutput.Text = "No suitable vertex cover could be found";
                return;
            }

            string edges = "";
            for(int i = 0; i < vertexCover.Length; i++)
            {
                if(vertexCover[i])
                {
                    edges += i+1 + " ";
                }
            }

            VertexCoverOutput.Text = "Vertices: " + edges + "form biggest vertex cover for graph";
        }

        private void DrawGraph(bool[,] adjacencyMatrix)
        {

            Graph graph = new Graph(adjacencyMatrix);
            GraphVizAttributes attributes =
                new GraphVizAttributes("my_graph", "Arial", "filled,setlinewidth(4)", "circle");

            for (int i = 0; i < graph.Vertices.Count; i++)
            {
                attributes.AddAttribute(graph.Vertices.ElementAt(i), new Tuple<string, string>("label", i.ToString()));
            }
            try
            {
                Uri location = GraphViz.CreateGraphImage($"graph{imagesGenerated++}", graph, attributes);
                ImageBox.Source = new BitmapImage(location);
            }
            catch (Win32Exception)
            {
                MessageBox.Show("Please install GraphViz at C:\\Program Files\\Graphviz\\bin\\dot.exe");
            }
            catch (OutOfMemoryException)
            {
                MessageBox.Show("The maximum size has been reached please create a smaller graph");
            }
        }

        private void GenerateDefaultGraph()
        {
            matrix = matrixBuilder.GenerateCompleteAdjacencyMatrix(5, 50);

            DrawGraph(matrix);
        }
    }
}