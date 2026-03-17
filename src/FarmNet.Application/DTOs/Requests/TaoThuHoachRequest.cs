namespace FarmNet.Application.DTOs.Requests;

public record TaoThuHoachRequest(Guid BatchId, DateTime NgayThuHoach, double TrongLuong, string? GhiChuChatLuong);
