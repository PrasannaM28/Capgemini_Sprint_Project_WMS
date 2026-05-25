namespace WMS.Application.DTOs.Client;

public class ClientResponseDto
{
    public int ClientId { get; set; }

    public string ClientName { get; set; }
        = string.Empty;

    public string? ClientLocation { get; set; }
}
