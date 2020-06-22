using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContinuumConcaveHull
{
    public class Hull
    {
        public List<Node> unused_nodes = new List<Node>();
        public List<Line> hull_edges = new List<Line>();



        public void makeHull(List<Node> nodes,double minAngle)
        {
            setConvexHull(nodes);
            setConcaveHull(minAngle);
            sortEdges();
        }

        private string dumpEdges()
        {
            List<String> diags = new List<string>();

            int count = 0;
            foreach(Line edge in hull_edges)
            {                
                diags.Add($"Item {count.ToString("##")}: "+ 
                    $"({edge.nodes[0].x},{edge.nodes[0].y})-({edge.nodes[1].x},{edge.nodes[1].y})");
                ++count;
            }

            string sDiag = string.Join("\r\n", diags);

            return sDiag;
        }

    

        // Pass in a list of nodes.
        // Get out a convex hull 
        private List<Line> getHull(List<Node> nodes)
        {
            List<Node> convexH = new List<Node>();
            List<Line> exitLines = new List<Line>();

            convexH.AddRange(GrahamScan.convexHull(nodes));
            for (int i = 0; i < convexH.Count - 1; i++)
            {
                exitLines.Add(new Line(convexH[i], convexH[i + 1]));
            }
            exitLines.Add(new Line(convexH[0], convexH[convexH.Count - 1]));
            return exitLines;
        }

        // Separate the nodes used in the convex hull from the un-used nodes.
        private void setConvexHull(List<Node> nodes)
        {
            unused_nodes.AddRange(nodes);
            hull_edges.AddRange(getHull(nodes));
            foreach (Line line in hull_edges)
            {
                foreach (Node node in line.nodes)
                {
                    unused_nodes.RemoveAll(a => a.id == node.id);
                }
            }
        }

        /*
        private void setConcaveHull(double minimumAngle)
        {
            Tuple<Line,Node,Double> currentMax = null;

            List<string> diags = new List<string>();

            int pass = 0;
            while ((currentMax = getMaxUnusedAngle()) != null && currentMax.Item3 > minimumAngle)
            {
                diags.Add($"Pass {pass.ToString("##")}; Line:[({currentMax.Item1.nodes[0].x},{currentMax.Item1.nodes[0].y})-({currentMax.Item1.nodes[1].x},{currentMax.Item1.nodes[1].y})]; Node:({currentMax.Item2.x},{currentMax.Item2.y}); Angle:{currentMax.Item3.ToString("#.##")}");

                // DIAG
                string edges = dumpEdges();
                // END DIAG

                // We have a point that should be made part of the concave hull
                convert(currentMax);

                // DIAG
                edges = dumpEdges();
                // END DIAG
                ++pass;
            }            
        }
        */

        // Original version; Before diagnostics were added.        
        private void setConcaveHull(double minimumAngle)
        {
            Tuple<Line,Node,Double> currentMax = null;

            while ((currentMax = getMaxUnusedAngle()) != null && currentMax.Item3 > minimumAngle)
            {
                // We have a point that should be made part of the concave hull
                convert(currentMax);
            }            
        }
         

        // DIAG
        /*
        private bool lineContainsNode(Line line, Node node)
        {
            return (
                (line.nodes[0].x == node.x && line.nodes[0].y == node.y) ||
                (line.nodes[1].x == node.x && line.nodes[1].y == node.y));
               
        }
        */
        // END DIAG

        private void convert(Tuple<Line,Node,Double> candidate)
        {
            hull_edges.Remove(candidate.Item1);

            string edges = dumpEdges();

            unused_nodes.Remove(candidate.Item2);

            Line newEdge1 = new Line(candidate.Item2, candidate.Item1.nodes[0]);
            Line newEdge2 = new Line(candidate.Item2, candidate.Item1.nodes[1]);

            hull_edges.Add(newEdge1);
            edges = dumpEdges();

            hull_edges.Add(newEdge2);
            edges = dumpEdges();
        }

        private Tuple<Line,Node,Double> getMaxUnusedAngle()
        {
            // For all lines and unused points, we want to find the greatest angle
            Tuple<Line, Node, Double> currentMax = null;

            //List<string> diags = new List<string>();

            foreach (Line line in hull_edges)
            {
                // We have 3 points, the two ends of the hull line and 
                // an un-used point.
                foreach (Node unused_node in unused_nodes)
                {
                    double angle = getAngle(
                        line.nodes[0].x, line.nodes[0].y,
                        line.nodes[1].x, line.nodes[1].y,
                        unused_node.x, unused_node.y);

                    // DIAG
                    //diags.Add($"Line:[({line.nodes[0].x},{line.nodes[0].y})-({line.nodes[1].x},{line.nodes[1].y})]; Node:({unused_node.x},{unused_node.y}); Angle:{angle.ToString("#.##")}");
                    // END DIAG

                    if (currentMax == null || angle > currentMax.Item3)
                        currentMax = new Tuple<Line, Node, Double>(line, unused_node, angle);

                }
            }
            
            // DIAG
            //string sDiag = string.Join("\r\n",diags);
            //string sCurrentMax = $"Line:[({currentMax.Item1.nodes[0].x},{currentMax.Item1.nodes[0].y})-({currentMax.Item1.nodes[1].x},{currentMax.Item1.nodes[1].y})]; Node:({currentMax.Item2.x},{currentMax.Item2.y}); Angle:{currentMax.Item3.ToString("#.##")}";
            // END DIAG

            return currentMax;
        }

        // Find angle at point 3
        private static double getAngle(
            double P1X, double P1Y,
            double P2X, double P2Y,
            double P3X, double P3Y)
        {
            // a = length P1-P3
            // b = length P2-P3
            // c = length P1-P2

            double a = getLength(P1X, P1Y, P3X, P3Y);
            double b = getLength(P2X, P2Y, P3X, P3Y);
            double c = getLength(P1X, P1Y, P2X, P2Y);

            double angleRad_P3 = Math.Acos((Math.Pow(a,2) + Math.Pow(b,2) - Math.Pow(c,2)) / (2 * a * b));
            double angleDeg_P3 = (angleRad_P3 * 180) / Math.PI;

            if (angleDeg_P3 < 0)
            {
                angleDeg_P3 = 180 + angleDeg_P3;
            }

            return angleDeg_P3;
        }

        private static double getLength(double P1X,double P1Y,double P2X, double P2Y)
        {
            return Math.Sqrt(
             Math.Pow((P1X - P2X), 2) +
             Math.Pow((P1Y - P2Y), 2)
             );
        }

        private static double getLength(Node a, Node b)
        {
            return Math.Sqrt(
                Math.Pow((a.x - b.x),2) + 
                Math.Pow((a.y-b.y),2)
                );
        }
        
        

        private void sortEdges()
        {
            List<Line> sorted_hull_edges = new List<Line>();

            Line lastEdge = hull_edges.Last();
            sorted_hull_edges.Add(lastEdge);
            hull_edges.Remove(lastEdge);

            while( hull_edges.Count > 0)
            {
                var connectingEdge = getEdgeWithNode(sorted_hull_edges.Last().nodes[0]);
                if (connectingEdge == null)
                    connectingEdge = getEdgeWithNode(sorted_hull_edges.Last().nodes[1]);

                sorted_hull_edges.Add(connectingEdge);
                hull_edges.Remove(connectingEdge);
            }

            hull_edges = sorted_hull_edges;
        }

        private Line getEdgeWithNode(Node node)
        {
            foreach (Line edge in hull_edges)
            {
                if (edge.nodes[0] == node) return edge;
                if (edge.nodes[1] == node) return edge;
            }

            // Fail
            return null;
        }

        private List<Line> getEdgesWithNode(Node node)
        {
            List<Line> edges = new List<Line>();

            foreach (Line edge in hull_edges)
            {
                if (edge.nodes[0] == node) edges.Add(edge);
                if (edge.nodes[1] == node) edges.Add(edge);
            }

            return edges;
        }

        private Node getCommonNode(Line a, Line b)
        {
            if (a.nodes[0].x == b.nodes[0].x && a.nodes[0].y == b.nodes[0].y) return a.nodes[0]; // 0 nodes match
            if (a.nodes[1].x == b.nodes[1].x && a.nodes[1].y == b.nodes[1].y) return a.nodes[1]; // 1 nodes match
            if (a.nodes[1].x == b.nodes[0].x && a.nodes[1].y == b.nodes[0].y) return a.nodes[1]; // a1 matches b0
            if (a.nodes[0].x == b.nodes[1].x && a.nodes[0].y == b.nodes[1].y) return a.nodes[0]; // a0 matches b1
            return null;
        }



        public List<Node> getWay()
        {
            // We need 3 edges minimum to make a polygon
            if (hull_edges.Count < 3)
                return null;

            List<Node> way = new List<Node>();

            var edge0 = hull_edges[0];
            var edge1 = hull_edges[1];

            var commonNode = getCommonNode(edge0, edge1);

            if (commonNode == null)
                return null;

            // The node that is not common is the starting node.
            Node startNode = null;
            Node nextNode = null;
            if (edge0.nodes[0].x == commonNode.x && edge0.nodes[0].y == commonNode.y)
            {
                startNode = edge0.nodes[1];
                nextNode = edge0.nodes[0];
            }
            else
            {
                startNode = edge0.nodes[0];
                nextNode = edge0.nodes[1];
            }

            way.Add(startNode);
            way.Add(nextNode);

            Node currentNode = nextNode;

            while ((currentNode = getNextNode(currentNode, way)) != null)
            {
                way.Add(currentNode);
            }

            // Add the starting node back as the final node to complete the way
            way.Add(startNode);

            return way;
        }



        private Node getNextNode(Node currentNode, List<Node> way)
        {
            foreach(Line edge in hull_edges)
            {
                bool edgeNode0IsInWay = way.Contains(edge.nodes[0]);
                bool edgeNode1IsInWay = way.Contains(edge.nodes[1]);

                if ((edgeNode0IsInWay && !edgeNode1IsInWay) || 
                    (!edgeNode0IsInWay && edgeNode1IsInWay))
                {
                    // A node in this edge is in, and the other is not.
                    // This edge is an extension of the current way.
                    if (edgeNode0IsInWay)
                        return edge.nodes[1];
                    else
                        return edge.nodes[0];
                    
                }               
            }

            // Fail
            return null;
        }

        public string dumpWay(List<Node> way)
        {
            List<string> rets = new List<string>();

            foreach (Node node in way)
            {
                rets.Add($"({node.x},{node.y})\r\n");
            }

            return string.Join("",rets);
        }
    }
}
