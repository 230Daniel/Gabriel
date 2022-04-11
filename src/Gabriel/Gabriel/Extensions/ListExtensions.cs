using System.Text;

namespace Gabriel.Extensions;

public static class ListExtensions
{
    public static List<string> GetCodePages(this List<string> items, int groupSize)
    {
        var groups = new List<string>();
        var group = new StringBuilder("```cs\n");
        var currentGroupSize = 0;

        foreach (var item in items)
        {
            group.Append(item);
            group.Append("\n\n");
            currentGroupSize++;

            if (currentGroupSize == groupSize)
            {
                currentGroupSize = 0;
                group.Append("```");
                groups.Add(group.ToString());
                group = new StringBuilder("```cs\n");
            }
        }

        if (currentGroupSize != 0)
        {
            group.Append("```");
            groups.Add(group.ToString());
        }

        return groups;
    }
}
