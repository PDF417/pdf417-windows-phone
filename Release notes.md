# Release notes

## 1.0.0

- fixed ZXing recognizer not obeying scanning region when recognising 1D barcodes in portrait orientation
- improved USDL barcode parsing
- fixed crash in USDL parser
- implemented DirectAPI for recognition of individual bitmaps
- added PDF417DirectAPIDemo sample app
- bitmaps can now be processed while RecognizerControl is active using method Recognize
- by default, null quiet zone is now set to true in US Driver's License recognizer

## 0.8.0

- Initial PDF417 version for Windows Phone 8.0