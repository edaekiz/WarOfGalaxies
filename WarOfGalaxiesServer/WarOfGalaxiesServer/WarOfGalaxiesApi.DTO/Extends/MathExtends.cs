using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WarOfGalaxiesApi.DTO.Extends
{
    public static class MathExtends
    {
        public static T GetRandom<T>(IEnumerable<T> values) => values.OrderBy(x => Guid.NewGuid()).FirstOrDefault();

    }
}
