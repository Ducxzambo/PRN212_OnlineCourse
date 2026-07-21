namespace Presentation.Helpers;

public class EnrollmentStatusOption
{
    public int Value { get; init; }
    public string Text { get; init; } = "";
}

public static class EnrollmentStatusOptions
{
    public static List<EnrollmentStatusOption> All { get; } = new()
    {
        new EnrollmentStatusOption { Value = 0, Text = "Đã đăng ký" },
        new EnrollmentStatusOption { Value = 1, Text = "Đang học" },
        new EnrollmentStatusOption { Value = 2, Text = "Đã hoàn thành" },
        new EnrollmentStatusOption { Value = 3, Text = "Đã hủy" },
    };
}

/// <summary>Used for the "filter by course" dropdown; CourseId == null means "all courses".</summary>
public class CourseFilterOption
{
    public int? CourseId { get; init; }
    public string DisplayName { get; init; } = "";
}
