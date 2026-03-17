namespace FarmNet.Application.DTOs.Requests;

public record TaoBatchRequest(string MaLo, string TenSanPham, string? MoTa, Guid FarmId);
