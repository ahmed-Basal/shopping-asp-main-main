using System;
using System.Collections.Generic;
using System.Text;

namespace core.Dto
{
    public  record CategoryDto(
        string name,
        string Description
        );
    public record updateRecordCategory(
        int id,
        string name,
        string Description
        );
}
