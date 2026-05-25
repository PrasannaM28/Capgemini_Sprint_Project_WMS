namespace WMS.Application.DTOs.Client;

public class CreateClientDto
{
    public string ClientName { get; set; }
        = string.Empty;

    public string? ClientAddress { get; set; }

    public string? ClientPhoneNumber { get; set; }

    public string? ClientLocation { get; set; }
}
