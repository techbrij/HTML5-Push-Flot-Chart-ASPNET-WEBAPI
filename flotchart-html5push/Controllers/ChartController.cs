using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using System.Net.Http.Headers;
using System.Net;
using System.Collections.Concurrent;
using System.Threading;
using Newtonsoft.Json;
using System.Diagnostics;


namespace TechBrij.HTML5push.Controllers
{
    public class ChartController : ApiController
    {
        private static readonly Lazy<Timer> _timer = new Lazy<Timer>(() => new Timer(TimerCallback, null, 0, 1000));
        private static readonly ConcurrentQueue<StreamWriter> _streammessage = new ConcurrentQueue<StreamWriter>();

        public HttpResponseMessage Get(HttpRequestMessage request)
        {
            Timer t = _timer.Value;                       
            HttpResponseMessage response = request.CreateResponse();
            response.Content = new PushStreamContent(OnStreamAvailable, "text/event-stream");
            return response;
        }


        private static void TimerCallback(object state)
        {
            Random randNum = new Random(); 
            foreach (var data in _streammessage)
            {

                data.WriteLine("data:" + randNum.Next(30, 100) + "\n");
                data.Flush();

            }
            //To set timer with random interval
            _timer.Value.Change(TimeSpan.FromMilliseconds(randNum.Next(1,3)*500), TimeSpan.FromMilliseconds(-1));

        }


        public static void OnStreamAvailable(Stream stream, HttpContentHeaders headers, TransportContext context)
        {
            StreamWriter streamwriter = new StreamWriter(stream);
            _streammessage.Enqueue(streamwriter);
        }
    }
}