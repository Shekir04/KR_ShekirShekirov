using System;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine("╔══════════════════════════════════════╗");
        Console.WriteLine("║   Event vs Делегат – Чат Система     ║");
        Console.WriteLine("╚══════════════════════════════════════╝\n");

        Scenario1_NormalUsage();
        Scenario2_AccidentalReset();
        Scenario3_InvokeFromOutside();
    }

    // --------------------------------------------------------
    // СЦЕНАРИЙ 1: Нормална употреба
    // Резултат: И двата работят ЕДНАКВО
    // --------------------------------------------------------
    static void Scenario1_NormalUsage()
    {
        Console.WriteLine("═══════════════════════════════════════");
        Console.WriteLine("СЦЕНАРИЙ 1: Нормална употреба");
        Console.WriteLine("═══════════════════════════════════════");

        // --- С делегат ---
        Console.WriteLine("\n▶ С обикновен делегат:");
        var delegateRoom = new DelegateChatRoom("Общ чат");

        // Абонираме потребители с +=
        delegateRoom.OnMessage += msg => Console.WriteLine($"  Алис получи: {msg}");
        delegateRoom.OnMessage += msg => Console.WriteLine($"  Боб получи:  {msg}");

        delegateRoom.SendMessage("Иван", "Здравейте!");
        delegateRoom.SendMessage("Мария", "Как сте?");

        // --- С event ---
        Console.WriteLine("\n▶ С event:");
        var eventRoom = new EventChatRoom("Общ чат");

        eventRoom.OnMessage += msg => Console.WriteLine($"  Алис получи: {msg}");
        eventRoom.OnMessage += msg => Console.WriteLine($"  Боб получи:  {msg}");

        eventRoom.SendMessage("Иван", "Здравейте!");
        eventRoom.SendMessage("Мария", "Как сте?");

        Console.WriteLine("\nРЕЗУЛТАТЪТ Е ЕДНАКЪВ И ПРИ ДВАТА ПОДХОДА.\n");
    }

    // --------------------------------------------------------
    // СЦЕНАРИЙ 2: Случайно нулиране на абонатите
    // Резултат: С делегат – съобщенията се губят!
    //           С event   – компилаторът не позволява грешката
    // --------------------------------------------------------
    static void Scenario2_AccidentalReset()
    {
        Console.WriteLine("═══════════════════════════════════════");
        Console.WriteLine("СЦЕНАРИЙ 2: Случайно нулиране");
        Console.WriteLine("═══════════════════════════════════════");

        // --- С ДЕЛЕГАТ ---
        Console.WriteLine("\n▶ С обикновен делегат:");
        var delegateRoom = new DelegateChatRoom("Работен чат");

        delegateRoom.OnMessage += msg => Console.WriteLine($"  Алис получи: {msg}");
        delegateRoom.OnMessage += msg => Console.WriteLine($"  Боб получи:  {msg}");

        delegateRoom.SendMessage("Иван", "Първо съобщение – всички го виждат");

        // Симулираме грешка: трети потребител случайно използва = вместо +=
        // Това ИЗТРИВА всички предишни абонати!
        Console.WriteLine("\n  Карло се абонира, но използва = вместо +=");
        delegateRoom.OnMessage = msg => Console.WriteLine($"  Карло получи: {msg}");

        delegateRoom.SendMessage("Иван", "Второ съобщение – кой го вижда?");
        Console.WriteLine("  Алис и Боб НЕ получиха второто съобщение!\n");

        // --- С EVENT ---
        Console.WriteLine("▶ С event:");
        var eventRoom = new EventChatRoom("Работен чат");

        eventRoom.OnMessage += msg => Console.WriteLine($"  Алис получи: {msg}");
        eventRoom.OnMessage += msg => Console.WriteLine($"  Боб получи:  {msg}");

        eventRoom.SendMessage("Иван", "Първо съобщение – всички го виждат");

        // Следният код НЕ се компилира с event:
        //eventRoom.OnMessage = msg => Console.WriteLine($"  Карло получи: {msg}");

        Console.WriteLine("\n  С event компилаторът не позволява = ");
        Console.WriteLine("  Грешката се хваща преди изпълнение!\n");
    }

    // --------------------------------------------------------
    // СЦЕНАРИЙ 3: Извикване на делегата/събитието отвън
    // Резултат: С делегат – може да се извика от всеки!
    //           С event   – само класът-собственик може да го извика
    // --------------------------------------------------------
    static void Scenario3_InvokeFromOutside()
    {
        Console.WriteLine("═══════════════════════════════════════");
        Console.WriteLine("СЦЕНАРИЙ 3: Извикване отвън");
        Console.WriteLine("═══════════════════════════════════════");

        // --- С делегат ---
        Console.WriteLine("\n▶ С обикновен делегат:");
        var delegateRoom = new DelegateChatRoom("VIP чат");

        delegateRoom.OnMessage += msg => Console.WriteLine($"  Потребител получи: {msg}");

        // Извикваме делегата директно отвън – заобикаляме SendMessage!
        // Така може да се изпратят съобщения без логика за валидация,
        // без да се запише в лог, без проверка на права, и т.н.
        Console.WriteLine("  Извикваме OnMessage директно (заобикаляме SendMessage):");
        delegateRoom.OnMessage?.Invoke("Фалшиво системно съобщение!");

        // --- С event ---
        Console.WriteLine("\n▶ С event:");
        var eventRoom = new EventChatRoom("VIP чат");

        eventRoom.OnMessage += msg => Console.WriteLine($"  Потребител получи: {msg}");

        // Следният код НЕ се компилира:
        //eventRoom.OnMessage.Invoke("Фалшиво съобщение");
        //eventRoom.OnMessage("Фалшиво съобщение");

        Console.WriteLine("  С event не може да се извика отвън.");
        Console.WriteLine("  Само SendMessage() може да го задейства.\n");

        Console.WriteLine("═══════════════════════════════════════");
        Console.WriteLine("ЗАКЛЮЧЕНИЕ:");
        Console.WriteLine("  Делегат = публична променлива (опасно)");
        Console.WriteLine("  Event   = защитена нотификация (препоръчително)");
        Console.WriteLine("═══════════════════════════════════════");
    }
}
