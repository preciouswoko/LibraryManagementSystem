using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(UserDto user);
        string GenerateToken1(UserDto user);
        bool ValidateToken(string token);
    }

}
