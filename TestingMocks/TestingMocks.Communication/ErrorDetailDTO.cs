namespace TestingMocks.Communication;

/// <summary>
/// Информация об ошибка
/// </summary>
/// <param name="Detail">Описание ошибки</param>
/// <param name="Status">HTTP-код ошибки</param>
public record ErrorDetailDTO(string? Detail, int Status);