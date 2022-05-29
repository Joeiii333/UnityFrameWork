using System;

namespace TeachTcpServerExercises2
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerSocket socket = new ServerSocket();
            socket.Start("127.0.0.1", 8080, 1024);
            Console.WriteLine("服务器开启成功");
            while (true)
            {
                string input = Console.ReadLine();
                if(input == "Quit")
                {
                    socket.Close();
                }
                else if( input.Substring(0,2) == "B:" )
                {
                    socket.Broadcast(input.Substring(2));
                }
            }
        }
    }
}
