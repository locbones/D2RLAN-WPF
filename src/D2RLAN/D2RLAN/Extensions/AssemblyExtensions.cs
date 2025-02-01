using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace D2RLAN.Extensions
{
    public static class AssemblyExtensions
    {
        public static async Task<string> ReadResourceAsync(this Assembly assembly, string name)
        {
            // Determine path
            string resourcePath = name;
            // Format: "{Namespace}.{Folder}.{filename}.{Extension}"
            if (!name.StartsWith(nameof(D2RLAN)))
            {
                resourcePath = assembly.GetManifestResourceNames()
                                       .Single(str => str.EndsWith(name));
            }

            await using Stream stream = assembly.GetManifestResourceStream(resourcePath)!;
            using StreamReader reader = new(stream);
            return await reader.ReadToEndAsync();
        }

        public static async Task<byte[]> ReadResourceByteArrayAsync(this Assembly assembly, string name)
        {
            // Determine path
            string resourcePath = name;
            // Format: "{Namespace}.{Folder}.{filename}.{Extension}"
            if (!name.StartsWith(nameof(D2RLAN)))
            {
                resourcePath = assembly.GetManifestResourceNames()
                                       .Single(str => str.EndsWith(name));
            }

            await using Stream stream = assembly.GetManifestResourceStream(resourcePath)!;

            MemoryStream memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }

        public static byte[] ReadResourceByteArrayAsync2(this Assembly assembly, string name)
        {
            // Determine path
            string resourcePath = name;
            // Format: "{Namespace}.{Folder}.{filename}.{Extension}"
            if (!name.StartsWith(nameof(D2RLAN)))
            {
                resourcePath = assembly.GetManifestResourceNames()
                                       .Single(str => str.EndsWith(name));
            }

            using Stream stream = assembly.GetManifestResourceStream(resourcePath)!;

            MemoryStream memoryStream = new MemoryStream();
            stream.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }
    }

}
