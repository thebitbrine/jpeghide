# jpeghide
Hide files inside images using steganography.

## What it does
jpeghide embeds any file inside JPEG images (and other formats) by appending compressed data to the end of the image file. The host image displays normally while secretly containing your hidden file.

The hidden file gets compressed with LZMA2 before embedding, significantly reducing the final file size.

## How it works
The tool appends your file to the end of the base image with a unique boundary marker. Since image viewers only read the actual image data, they ignore the extra bytes at the end. The image displays perfectly while containing your hidden payload.

**Compression**: Files are compressed with LZMA2 before hiding, often reducing size by 50-90% depending on the file type.

**Boundary detection**: Uses a 64-character marker to separate the image from hidden data during extraction.

## Supported formats
- **Base files**: JPEG, PNG, TIFF, MP3, WAV, or any format with defined file boundaries
- **Hidden files**: Any file type (documents, executables, archives, media, etc.)
- **Text files**: Can be used as base files but will show obvious size increases

## Usage

Run the program and choose:
- **F1** - Hide file (embed file into image)
- **F2** - Extract file (recover hidden file from image)

### Hiding a file
```
Press [F1] for hiding
Enter base image path: C:\photo.jpg
Enter quote file path: C:\secret_document.pdf
Enter destination file path: C:\output_photo.jpg
```

### Extracting a file
```
Press [F2] for discovery  
Enter base image path: C:\output_photo.jpg
Enter destination file path: C:\recovered_document.pdf
```

## Why this approach works
Most file formats have specific end markers. By appending data after these markers, the original file remains fully functional while carrying additional payload. 

Image viewers, media players, and document readers only process the original file structure and ignore trailing data.

## Technical details
- **Compression**: LZMA2 algorithm with property headers and file length encoding
- **Boundary marker**: 64-character unique string prevents false detection
- **File integrity**: Perfect reconstruction guaranteed - no data loss
- **Size overhead**: Only compressed file size + 64 bytes for marker
- **Detection resistance**: Hidden data appears as random bytes at file end

## Advantages
- **Invisible**: Host image displays identically to original
- **Universal**: Works with most file formats as both base and payload
- **Efficient**: High compression ratios reduce final file size
- **Simple**: No complex image manipulation or pixel modifications
- **Reliable**: Deterministic extraction with boundary detection

## Requirements
- .NET Framework
- LZMA-SDK for compression
- Read/write access to input and output files

## Limitations
- File size increases by compressed payload size
- Not suitable for scenarios requiring pixel-level steganography
- Boundary marker could theoretically appear in legitimate data (extremely unlikely)
- Some file validation tools may detect appended data

## Security note
This provides data hiding, not encryption. For sensitive data, encrypt files before hiding them. The technique is primarily for concealment rather than cryptographic protection.

The approach offers a practical balance between simplicity, reliability, and effectiveness for general steganographic applications.
