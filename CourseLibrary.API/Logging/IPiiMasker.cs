namespace CourseLibrary.API.Logging
{
    public interface IPiiMasker
    {
        string Mask(string input);
    }
}
