namespace MoviePlatformApi.ClassExtension
{
    public static class IFormFileExtensions
    {
        public static byte[] ToByteArray(this IFormFile i)
        {
            using (BinaryReader sr = new BinaryReader(i.OpenReadStream()))
            {
                return sr.ReadBytes((int)i.Length);
            }
        }
    }
}