using PostPilot.Api.Features.Categories.Dtos;

namespace PostPilot.Api.Features.Categories;

internal static class CategoryTagInputMapper
{
    public static IReadOnlyList<(string TagText, int SortOrder)> ToTagInputs(IEnumerable<CategoryTagRequestDto>? tags)
    {
        var result = new List<(string TagText, int SortOrder)>();

        if (tags is null)
        {
            return result;
        }

        var index = 1;
        foreach (var tag in tags)
        {
            if (string.IsNullOrWhiteSpace(tag.TagText))
            {
                index++;
                continue;
            }

            result.Add((tag.TagText.Trim(), tag.SortOrder ?? index));
            index++;
        }

        return result;
    }
}