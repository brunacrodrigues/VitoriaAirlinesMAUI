using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VitoriaAirlinesMAUI.Model;
using VitoriaAirlinesMAUI.Services.Interfaces;

namespace VitoriaAirlinesMAUI.ViewModel;

[QueryProperty(nameof(TicketId), "TicketId")]
public partial class BoardingPassViewModel : BaseViewModel
{
    private readonly IBookingService _bookingService;
    private readonly IApiService _apiService;

    [ObservableProperty] private int ticketId;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasBoardingPass))]
    [NotifyPropertyChangedFor(nameof(IsDownloadEnabled))]
    private BoardingPass? boardingPass;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsDownloadEnabled))]
    private bool isDownloading = false;


    // Computed properties for UI visibility
    public bool HasBoardingPass => BoardingPass != null;
    public bool IsDownloadEnabled => !IsDownloading;

    public BoardingPassViewModel(IBookingService bookingService, IApiService apiService)
    {
        _bookingService = bookingService;
        _apiService = apiService;
        Title = "Your Boarding Pass";
    }


    partial void OnTicketIdChanged(int value)
    {
        if (value > 0)
        {
            _ = LoadBoardingPassAsync(value);
        }
    }


    private async Task LoadBoardingPassAsync(int tId)
    {
        IsBusy = true;
        try
        {
            System.Diagnostics.Debug.WriteLine($"[BoardingPassVM] Loading ticket {tId}...");

            var apiResponse = await _bookingService.GetBoardingPassAsync(tId);

            if (apiResponse.IsSuccess && apiResponse.Data != null)
            {
                BoardingPass = apiResponse.Data;
            }
            else
            {

                await Shell.Current.DisplayAlert("Error", apiResponse.Message ?? "Failed to load boarding pass.", "OK");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[BoardingPassVM] EXCEPTION: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }



    [RelayCommand]
    private async Task DownloadTicketAsync()
    {
        if (IsBusy || IsDownloading || TicketId <= 0) return;

        IsDownloading = true;
        try
        {
            var response = await _apiService.GetStreamAsync($"api/tickets/{TicketId}/download-pdf");

            if (response.IsSuccess && response.Data != null)
            {
                // Generate filename
                var fileName = $"VitoriaAirlines-Ticket-{TicketId}.pdf";

                // Save to device using MAUI's file system
                await SavePdfToDevice(response.Data, fileName);

                await Shell.Current.DisplayAlert("Download Complete",
                    $"Your boarding pass has been saved as {fileName}", "OK");
            }
            else
            {
                await Shell.Current.DisplayAlert("Download Failed",
                    response.Message ?? "Could not download the boarding pass.", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Download Error",
                $"An error occurred while downloading: {ex.Message}", "OK");
        }
        finally
        {
            IsDownloading = false;
        }
    }


    private async Task SavePdfToDevice(Stream pdfStream, string fileName)
    {
        try
        {
#if ANDROID
            var context = Platform.CurrentActivity ?? throw new InvalidOperationException("Android Context is null.");
            var contentResolver = context.ContentResolver;

            // 2. Usar MediaStore para criar uma nova entrada no diretório Downloads
             var contentValues = new Android.Content.ContentValues();
            contentValues.Put(Android.Provider.MediaStore.IMediaColumns.DisplayName, fileName);
            contentValues.Put(Android.Provider.MediaStore.IMediaColumns.MimeType, "application/pdf");
            contentValues.Put(Android.Provider.MediaStore.IMediaColumns.RelativePath, Android.OS.Environment.DirectoryDownloads);
            
            // 3. Inserir e obter a URI do novo ficheiro
             var uri = contentResolver.Insert(Android.Provider.MediaStore.Downloads.ExternalContentUri, contentValues);
            
            if (uri == null)
            {
                throw new Exception("Android ContentResolver failed to create a new file URI.");
            }
            
            // 4. Abrir um OutputStream e copiar o PDF Stream
            using (var outputStream = contentResolver.OpenOutputStream(uri))
            {
                await pdfStream.CopyToAsync(outputStream);
            }
#elif IOS
            // iOS: Save to Documents folder (no permissions needed)
            var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var filePath = Path.Combine(documentsPath, fileName);

            using var fileStream = File.Create(filePath);
            await pdfStream.CopyToAsync(fileStream);

            // Optional: Share the file with the user so they can save it to Files app
            await SharePdfFile(filePath);
#else
            // Other platforms: use app data directory
            var localAppData = FileSystem.Current.AppDataDirectory;
            var filePath = Path.Combine(localAppData, fileName);

            using var fileStream = File.Create(filePath);
            await pdfStream.CopyToAsync(fileStream);
#endif
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to save PDF: {ex.Message}", ex);
        }
    }

#if IOS
    private async Task SharePdfFile(string filePath)
    {
        try
        {
            // Share the PDF so user can save to Files app or other locations
            await Share.RequestAsync(new ShareFileRequest
            {
                Title = "Save Boarding Pass",
                File = new ShareFile(filePath)
            });
        }
        catch
        {
            // Silently fail - file is still saved in Documents
        }
    }
#endif
}