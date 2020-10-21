using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Rivals
{
	public class RivalsTask
	{
		public static IEnumerable<OwnedLocation> AssignOwners(Map map)
		{
			var vizited = new Dictionary<Point, OwnedLocation>();
			var queue = MakeOwnersQueue(map);

			while (queue.Count > 0)
			{
				var current = queue.Dequeue();

				if (vizited.ContainsKey(current.Location) || IsInvalidLocation(current, map))
					continue;

				vizited[current.Location] = current;
				yield return current;
				MarksAround(queue, current);
			}
		}

		private static void MarksAround(Queue<OwnedLocation> queue, OwnedLocation current)
		{
			//for (var dy = -1; dy <= 1; dy++)
			//	for (var dx = -1; dx <= 1; dx++)
			//		if ((dy == 0 || dx == 0) && dy != dx)
			//		{
			//			var next = new OwnedLocation(current.Owner,
			//				new Point(current.Location.X + dx, current.Location.Y + dy),
			//				current.Distance + 1);
			//			queue.Enqueue(next);
			//		}

			var range = Enumerable.Range(-1, 3);

			var marks = range.SelectMany(dx => range.Select(dy => (dx, dy)))
				.Where(loc => (loc.dx == 0 || loc.dy == 0) && loc.dx != loc.dy)
				.Select(loc => new OwnedLocation(
					current.Owner,
					new Point(current.Location.X + loc.dx, current.Location.Y + loc.dy),
					current.Distance + 1));
			foreach (var mark in marks)
				queue.Enqueue(mark);
		}

		private static Queue<OwnedLocation> MakeOwnersQueue(Map map)
		{
			var queue = new Queue<OwnedLocation>();
			for (int owner = 0; owner < map.Players.Length; owner++)
				queue.Enqueue(new OwnedLocation(owner, map.Players[owner], 0));
			return queue;
		}

		private static bool IsInvalidLocation(OwnedLocation current, Map map) =>
			!map.InBounds(current.Location) ||
			map.Maze[current.Location.X, current.Location.Y] != MapCell.Empty;
	}
}
