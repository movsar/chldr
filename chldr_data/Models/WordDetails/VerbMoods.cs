using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chldr_data.Models.WordDetails
{
    public class VerbMoods
    {
        // Наклонение
        public string Indicative { get; set; } = string.Empty;
        public string Imperative { get; set; } = string.Empty;
        public string InterrogativeIndicative { get; set; } = string.Empty;
        public string RealConditional { get; set; } = string.Empty;
        public string UnrealConditional { get; set; } = string.Empty;
        public string RealDesiderative { get; set; } = string.Empty;
        public string UnrealDesiderative { get; set; } = string.Empty;
    }
}

/*
 1. Изъявительное наклонение – БИЛГАЛА САТТАМ
2. Повелительное наклонение – ТIЕДОЖОРОН САТТАМ
3. Вопросительная форма изъявительного наклонения – БИЛГАЛА САТТАМАН ХАТТАРАН КЕП
4. Реально-условное наклонение (обстоятельственная форма глагола) –БАКЪ БОЛУ БЕХКАМАН САТТАМ
5. Нереально-условное наклонение –БАКЪ БОЦУ БЕХКАМАН САТТАМ
6. Реально-желательное наклонение – БАКЪ БОЛУ ЛААРАН САТТАМ
7. Нереально-желательное наклонение – БАКЪ БОЦУ ЛААРАМ САТТАМ 

Indicative mood - BILGALA SATTAM (statement)
Imperative mood - TIYEDOJORON SATTAM (command)
Interrogative indicative mood - BILGALA SATTAMAN HATTARAN KEP (question/statement)
Real conditional mood (verbal adverbial form) - BAKY BOLU BEHKAMAN SATTAM (if...then)
Unreal conditional mood - BAKY BOTSU BEHKAMAN SATTAM (if...then)
Real desiderative mood - BAKY BOLU LAARAN SATTAM (hope/wish)
Unreal desiderative mood - BAKY BOTSU LAARAM SATTAM (hope/wish)



 */