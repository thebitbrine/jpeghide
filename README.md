# jpeghide
### Basic JPEG stenography implementation in C# 
Hide files in JPEG images.

- Added LZMA2 compression to quote files.
- Added support for PNG, TIFF, MP3, WAV etc.

Note:
As it writes quote file data to the end of the base file with a defined border,
It can work with virtually any file as base and as quote as long as the base file has its own defined borders.
Otherwise the base file might not work as intended, but the quote file will still be recoverable through jpeghide.

Text files can still be used as base, however it will be obvious to others that it contains some additional data.
