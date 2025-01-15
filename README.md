# MagicBytesValidator
Recognize file types from `Stream`s or `IFormFile`s using MIME types or file extensions and validate them against the magic bytes according to the file types.
The existing `IFileType`s can be expanded in various ways.

### How to install?

- Install nuget package into your project:

```powershell
Install-Package MagicBytesValidator -Version 2.0.3
```

```bash
dotnet add package MagicBytesValidator --version 2.0.3
```

- Reference in your csproj:
```xml
<PackageReference Include="MagicBytesValidator" Version="2.0.3" />
```

### How to use it?

- Create new instances of the validator and providers:
```c#
var validator = new MagicBytesValidator.Services.Validator();
var formFileTypeProvider = new MagicBytesValidator.Services.Http.FormFileTypeProvider();
var streamFileTypeProvider = new MagicBytesValidator.Services.Streams.StreamFileTypeProvider();
```

- Find a file type by extension or MIME type:
```c#
var pngFileType = validator.Mapping.FindByExtension("png");
var pdfFileType = validator.Mapping.FindByMimeType("application/pdf");
```
- Determine and validate a file type by uploaded IFormFile:
```c#
var fileType = await formFileTypeProvider.FindValidatedTypeAsync(formFile, null, CancellationToken.None);
```

- Determine the file type of a file by its stream
```c#
var fileType = await streamFileTypeProvider.TryFindUnambiguousAsync(fileStream, CancellationToken.None);
```

- Check a file with its stream and file type:
```c#
var isValid = await validator.IsValidAsync(memoryStream, fileType, CancellationToken.None);
```

#### Expand the file type mapping

- Get mapping:
```c#
// use the validator:
var mapping = validator.Mapping;

// use the formFileTypeProvider:
var mapping = formFileTypeProvider.Mapping;

// or create a new instance of the mapping:
var mapping = new MagicBytesValidator.Services.Mapping(); 
```

- Register a single `FileByteFilter`:
```c#
mapping.Register(
    new FileByteFilter(
        "traperto/trp", // MIME type
        new[] { "trp" } // file extensions
    ) {
        // magic byte sequences
        StartsWith([
            0x78, 0x6c, 0x2f, 0x5f, 0x72, 0x65
        ])
        .EndsWith([
            0xFF, 0xFF
        ])
    }
)
```

- `FileByteFilter`s with specific offset checks:
```c#
mapping.Register(
    new FileByteFilter(
        "traperto/trp", // MIME type
        new[] { "trp" } // file extensions
    ) {
        // magic byte sequences
        Specific(new ByteCheck(512, [0xFD]));
    }
)
```
`ByteCheck` allows for negative offset values to look for a specific offset counting from the end of file.

- Register a list of filetypes:
```c#
mapping.Register(listOfFileTypes);
```

You can also create variants of `IFileType` and register them by passing the Assembly of the new `IFileType`s, e.g.
`mapping.Register(typeof(CustomFileType).Assembly);`. This will register all `IFileType`s of the given assembly that are also not abstract and have an empty constructor.

```c#
public class CustomFileType : FileTypeWithStartSequences
{
    public CustomFileType() : base(
        "traperto/trp",  // MIME type
        new[] { "trp" }, // file extensions
        new[] {          // magic byte sequences
            new byte[] { 0x74, 0x72, 0x61, 0x70, 0x65, 0x72, 0x74, 0x6f }
        }
    )
    {
    }
}

var assembly = typeof(CustomFileType).Assembly;
_mapping.Register(assembly);
```

### CLI
There's a CLI tool (_MagicBytesValidator.CLI_) which can be used to determine
MIME types for a local file by calling the following command:

```shell
dotnet run --project MagicBytesValidator.CLI -- [PATH]
```

This can be useful when debugging or validating newly added FileTypes.

### List of file types

|FileType|Extensions|MIME Types|
|-|-|-|
|AIF|aif, aiff, aifc|audio/x-aiff|
|BIN|bin, file, com, class, ini|application/octet-stream|
|BMP|bmp|image/bmp|
|CAB|cab|application/vnd.ms-cab-compressed, application/x-cab-compressed|
|DOC|doc, dot|application/msword|
|DOCX|docx|application/vnd.openxmlformats-officedocument.wordprocessingml.document|
|DXR|dxr, dcr, dir|application/x-director|
|EXE|exe, com, dll, drv, pif, qts, qtx , sys, acm, ax, cpl, fon, ocx, olb, scr, vbx, vxd, mui, iec, ime, rs, tsp, efi|application/x-dosexec, application/x-msdos-program|
|GIF|gif|image/gif|
|GZ|gz|application/gzip|
|HEIC|heic, heif|image/heic, image/heif|
|ICO|ico|image/x-icon|
|JPG|jpg, jpeg, jpe, jif, jfif, jfi|image/jpeg|
|MIDI|midi, mid|audio/x-midi|
|MP3|mp3|audio/mpeg|
|MP4|mp4|video/mp4|
|MPG|mpg, mpeg, mpe, m2p, vob|video/mpeg|
|ODP|odp|application/vnd.oasis.opendocument.presentation|
|ODS|ods|application/vnd.oasis.opendocument.spreadsheet|
|ODT|odt|application/vnd.oasis.opendocument.text|
|OGV|ogv, ogg, oga|video/ogg|
|PBM|pbm|image/x-portable-bitmap|
|PDF|pdf|application/pdf|
|PGM|pgm|image/x-portable-graymap|
|PNG|png|image/png|
|PPM|ppm|image/x-portable-pixmap|
|PPT|ppt, ppz, pps, pot|application/mspowerpoint, application/vnd.ms-powerpoint|
|PPTX|pptx|application/vnd.openxmlformats-officedocument.presentationml.presentation|
|RAR|rar|application/vnd.rar, application/x-rar-compressed|
|RPM|rpm|application/x-rpm, application/x-redhat-package-manager|
|RTF|rtf|application/rtf|
|SND|snd, au|audio/basic|
|SVG|svg, svgz|image/svg+xml|
|SWF|swf|application/x-shockwave-flash|
|3GP|3gp|video/3gpp|
|TIF|tif, tiff|image/tiff|
|TSV|ts, tsv, tsa, mpg, mpeg|video/mp2t|
|TXT|txt|text/plain|
|WEBM|mkv, mka, mks, mk3d, webm|video/webm|
|XLS|xls, xla|application/msexcel|
|XLSX|xlsx|application/vnd.openxmlformats-officedocument.spreadsheetml.sheet|
|XML|xml|application/xml, text/xml|
|Z|z|application/x-compress|
|ZIP|zip|application/zip, application/x-zip-compressed|

### What is the licence?

[MIT License](./LICENSE)

```
▓▓  ▓▓▓▓▓▓▓▓▓▓
▓▓          ▓▓
▓▓▓▓▓▓▓▓▓   ▓▓
▓▓          ▓▓            traperto GmbH
▓▓  ▓▓▓▓▓▓▓▓▓▓
▓▓
▓▓▓▓▓▓▓▓▓   ▓▓
```