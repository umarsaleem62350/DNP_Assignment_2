namespace Entities.DTOs;

public class SearchUserParametersDto
{
    public int? UsernameContains { get;  }

    public SearchUserParametersDto(int? usernameContains)
    {
        UsernameContains = usernameContains;
    }
}