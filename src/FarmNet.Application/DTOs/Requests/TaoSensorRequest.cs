namespace FarmNet.Application.DTOs.Requests;

public record TaoSensorRequest(string Ten, string? MoTa, Guid BatchId);
