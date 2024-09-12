using Godot;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;


class Graph {
	private Dictionary<(int,int), List<(int,int,double)>> adjacencyList;

	public Graph(){
		adjacencyList = new Dictionary<(int,int), List<(int,int,double)>>();
	}

	public void AddVertex((int,int) vertex)
	{
		if (!adjacencyList.ContainsKey(vertex))
		{
			adjacencyList[vertex] = new List<(int,int,double)>();
		}
	}

	public void AddEdge((int,int) vertex1, (int,int) vertex2, double weight){
		if (adjacencyList.ContainsKey(vertex1))
		{
			adjacencyList[vertex1].Add((vertex2.Item1,vertex2.Item2, weight));
		}
		else
		{
			AddVertex(vertex1);
			adjacencyList[vertex1].Add((vertex2.Item1,vertex2.Item2, weight));
		}

		if (adjacencyList.ContainsKey(vertex2))
		{
			adjacencyList[vertex2].Add((vertex1.Item1,vertex1.Item2, weight));
		}
		else
		{
			AddVertex(vertex2);
			adjacencyList[vertex2].Add((vertex1.Item1,vertex1.Item2, weight));
		}
	}

	public Dictionary<(int,int), List<(int,int,double)>> GetAdjacencyList(){
		return adjacencyList;
	}

	

}

public partial class TileMapLayer : Godot.TileMapLayer
{
	private Timer bomb_timer;
	const int SIZE = 8;

	readonly List<Vector2I> EXPLOSION_TILES = new List<Vector2I> {
		new Vector2I(1, 1),
		new Vector2I(1, 0),
		new Vector2I(1, -1),
		new Vector2I(0, 1),
		new Vector2I(0, -1),
		new Vector2I(-1, -1),
		new Vector2I(-1, 1),
		new Vector2I(-1, 0)
	};
	
	private Godot.TileMapLayer path_layer;

	
	Graph graph = new();

	Random rand = new();
	public override void _Ready()
	{
		bomb_timer = GetNode<Timer>("BombTimer");

		bomb_timer.WaitTime = 5;

		bomb_timer.Timeout += () => createBomb();

		path_layer = GetNode<Godot.TileMapLayer>("PathLayer");

		SignalBus.Instance.BombExploded += pos => Explode(pos);

		CreateGraph();

		var start = (0, 0);
		var (distances, paths) = Dijkstra(graph, start);


		List<(int,int)> maze = new();
		for (int i = 0; i< SIZE * 2 + 1; i++){
			for (int j = 0; j< SIZE * 2 + 1; j++){
				if (!(i % 2 == 1 && j % 2 == 1)){
					maze.Add((i,j));
				}
			}
		}

		foreach (var node in paths.Keys){
			
			var parent = paths[node];

			//Adicionar exdpolicacao
			
			if(parent != null){
				(int,int) cell_to_remove;
				int x;
				int y;
				if(parent.Value.Item1 == node.Item1){
					if (parent.Value.Item2 > node.Item2){
						x = parent.Value.Item1 * 2 + 1;
						y = parent.Value.Item2 * 2;
					}else{
						x = node.Item1 * 2 + 1;
						y = node.Item2 * 2;
					}
					cell_to_remove = (x,y);
				} else {
					if (parent.Value.Item1 > node.Item1){
						y = parent.Value.Item2 * 2 + 1;
						x = parent.Value.Item1 * 2;
					}else {
						y = node.Item2 * 2 + 1;
						x = node.Item1 * 2;
					}
					cell_to_remove = (x,y);
				}

				maze.Remove(cell_to_remove);
			}
		}

		var target = ( SIZE-1, SIZE-1);

		var path_to_exit = GetPathGraph(target, paths);

		foreach (var cell in maze){
			this.SetCell(new Vector2I(cell.Item1,cell.Item2), 1, Vector2I.Zero, 1);
			
		}

		foreach(var path in path_to_exit){
			path_layer.SetCell(new Vector2I(path.Item1 * 2 + 1,path.Item2 * 2 + 1), 1, Vector2I.Zero, 2);
		}

		bomb_timer.Start();

	}

	private void createBomb(){
		var x = rand.Next(1,SIZE);
		var y = rand.Next(1, SIZE);
		var bomb_position = new Vector2I(x % SIZE * 2 + 1, y % SIZE * 2 + 1);

		if (this.GetCellSourceId(bomb_position) == -1){
			this.SetCell(bomb_position, 1, Vector2I.Zero, 3);
		}


	}

	private void Explode(Vector2 pos){
		var explosion_spot = new Vector2I((int)Math.Floor(pos.X / 64),(int) Math.Floor(pos.Y / 64));
		
		foreach(Vector2I v in EXPLOSION_TILES){
			this.SetCell(explosion_spot + v, -1, Vector2I.Zero, -1);
		}
		this.SetCell(explosion_spot, -1, Vector2I.Zero, -1);
		
	}

	private static (Dictionary<(int, int), double>, Dictionary<(int, int), (int, int)?>) Dijkstra(Graph graph, (int, int) start)
	{
		var distances = new Dictionary<(int, int), double>();
		var previous = new Dictionary<(int, int), (int, int)?>();
		var priorityQueue = new SortedSet<((int, int), double)>(Comparer<((int, int), double)>.Create((a, b) => a.Item2.CompareTo(b.Item2)));
		var adjacencyList = graph.GetAdjacencyList();

		// Initialize distances to all vertices as infinite
		foreach (var vertex in adjacencyList.Keys)
		{
			distances[vertex] = double.MaxValue;
			previous[vertex] = null;
		}

		distances[start] = 0;
		priorityQueue.Add((start, 0));

		while (priorityQueue.Count > 0)
		{
			var current = priorityQueue.Min.Item1;
			priorityQueue.Remove(priorityQueue.Min);

			foreach (var neighbor in adjacencyList[current])
			{
				(int x, int y, double weight) = neighbor;
				var neighborPosition = (x, y);
				double alt = distances[current] + weight;

				if (alt < distances[neighborPosition])
				{
					priorityQueue.Remove((neighborPosition, distances[neighborPosition]));
					distances[neighborPosition] = alt;
					previous[neighborPosition] = current;
					priorityQueue.Add((neighborPosition, alt));
				}
			}
		}

		return (distances, previous);
	}

	private void CreateGraph(){
		for (int i = 0; i< SIZE; i++){
			for (int j = 0; j< SIZE; j++){
				graph.AddVertex((i,j));
			}
		}

		var adjacencyList = graph.GetAdjacencyList();

		foreach(var x in adjacencyList) {
			int a = x.Key.Item1;
			int b = x.Key.Item2;

			int ap = a + 1;
			int bp = b + 1;
			int am = a - 1;
			int bm = b - 1;

			if (!(ap >= SIZE)){
				graph.AddEdge(x.Key, (ap,b), rand.NextDouble());
			}
			if (!(bm < 0 )){
				graph.AddEdge(x.Key, (a,bm), rand.NextDouble());
			}
			if (!(bp >= SIZE)){
				graph.AddEdge(x.Key, (a,bp), rand.NextDouble());
			}
			if (!(am < 0)){
				graph.AddEdge(x.Key, (am,b), rand.NextDouble());
			}

		}

	}
	private static List<(int,int)> GetPathGraph((int,int)? target, Dictionary<(int, int), (int, int)?> paths){
		var parent = target;
		List<(int,int)> path_to_exit = new();
		while(parent != null){
			path_to_exit.Add(((int, int))parent);
			parent = paths.ContainsKey(parent.Value) ? paths[parent.Value] : null;		
		}
		return path_to_exit;
	}

}
