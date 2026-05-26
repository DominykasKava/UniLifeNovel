using System.Collections.Generic;
using UnityEngine;

public class SprintCeremonyDescription : MonoBehaviour
{
    public static List<string> Planning = new List<string>()
    {
        "Sprint planning metu komanda susirenka aptarti būsimo sprinto tikslus ir užduotis.",
        "Product backlog elementai yra peržiūrimi ir prioritetizuojami.",
        "Kiekviena užduotis yra aiškinama, kad visi komandos nariai suprastų jos apimtį.",
        "Komanda įvertina, kiek darbo gali atlikti per sprintą.",
        "Sudarytas sprint backlog sąrašas su konkrečiomis užduotimis.",
        "Kiekvienas komandos narys žino savo atsakomybes.",
        "Planning procesas padeda užtikrinti aiškų darbo planą.",
        "Tai yra pagrindas visam sprinto darbui."
    };

    public static List<string> Daily = new List<string>()
    {
        "Daily susitikimai vyksta kiekvieną dieną tuo pačiu metu.",
        "Kiekvienas komandos narys trumpai pristato savo progresą.",
        "Aptariama, kas buvo padaryta vakar.",
        "Apibrėžiami dienos tikslai ir planai.",
        "Aptariamos kliūtys, kurios trukdo darbui.",
        "Komanda gali greitai prisitaikyti prie pokyčių.",
        "Daily padeda palaikyti darbo tempą.",
        "Užtikrinamas nuolatinis komunikavimas."
    };

    public static List<string> Review = new List<string>()
    {
        "Sprint review metu pristatomas atliktas darbas.",
        "Demonstruojamas funkcionalumas.",
        "Gaunamas grįžtamasis ryšys iš suinteresuotų šalių.",
        "Vertinama ar sprinto tikslas buvo pasiektas.",
        "Identifikuojami patobulinimai.",
        "Komanda aptaria rezultatus.",
        "Review leidžia įvertinti progreso kokybę.",
        "Padeda gerinti produktą."
    };

    public static List<string> Retrospective = new List<string>()
    {
        "Retrospective metu komanda analizuoja savo darbo procesą.",
        "Aptariama, kas sekėsi gerai.",
        "Identifikuojamos problemos.",
        "Ieškoma būdų pagerinti darbą.",
        "Kiekvienas komandos narys gali išreikšti nuomonę.",
        "Kuriami konkretūs veiksmų planai.",
        "Procesas padeda tobulėti kaip komandai.",
        "Užtikrinamas nuolatinis progresas."
    };
}