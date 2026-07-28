# ProxyOverlay

>[!IMPORTANT]
>Yes, this is in parts vibecoded. I wanted to get this done asap so I can start making Magic Proxies. 
>Maybe I'll clean it up at some point, but it's such a small app, that the slop in here is fine. ...probably

ProxyOverlay is a desktop application for applying a visual overlay to a folder of images. It is designed for batch image processing, with a live preview, selectable input and output folders, and configurable overlay scaling.

## Features

- Batch-process images in a folder
- Apply an overlay to each image
- Preview an image with the overlay before processing
- Choose a custom overlay file
- Scale the overlay from 10% to 100%
- Supports `.jpg`, `.jpeg`, `.png`, `.bmp`, `.gif`, `.webp`, `.tif`, and `.tiff`

## Installation

Either build it from source or download the pre-built executable from the [releases page](https://github.com/vibecode/ProxyOverlay/releases).

## Usage

1. Select the folder containing the source images.
2. Select an output folder.
3. Select the overlay image to apply.
4. Adjust the overlay size with the scale slider.
5. Check the preview and click **Start Processing**.

Processed images are written to the output folder using their original filenames. Existing files with the same names may be overwritten.


## Contributing Requirements

- .NET 10 SDK

The project currently targets `net10.0` and uses Avalonia 12 and ShadUi for its user interface.

## Getting started

Clone the repository and run the application from the repository root:

```powershell
git clone https://github.com/<your-account>/ProxyOverlay.git
cd ProxyOverlay
dotnet run --project .\ProxyOverlay\ProxyOverlay.csproj
```

To build a release version:

```powershell
dotnet build .\ProxyOverlay.slnx -c Release
```

## Project structure

```text
ProxyOverlay/
|- Assets/       Bundled overlay, preview, and application icon assets
|- Models/       Application models
|- Services/     File handling, image processing, and preview generation
|- ViewModels/   MVVM application logic
`- Views/        Avalonia UI / Window configurations (think XAML files)
```

## License

This project is licensed under the [MIT License](LICENSE.md).
