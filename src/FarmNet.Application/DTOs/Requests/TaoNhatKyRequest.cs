namespace FarmNet.Application.DTOs.Requests;

public record TaoNhatKyRequest(Guid BatchId, string HoatDong, string? GhiChu, DateTime NgayThucHien);
