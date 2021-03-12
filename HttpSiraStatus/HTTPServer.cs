using HttpSiraStatus.Interfaces;
using System;
using System.Text;
using WebSocketSharp.Server;
using Zenject;

namespace HttpSiraStatus
{
    public class HTTPServer : IInitializable, IDisposable
    {
        private int ServerPort = 6557;

        private HttpServer server;
        [Inject]
        private IStatusManager statusManager;
        private bool disposedValue;

        public void OnHTTPGet(HttpRequestEventArgs e)
        {
            var req = e.Request;
            var res = e.Response;

            if (req.RawUrl == "/status.json") {
                res.StatusCode = 200;
                res.ContentType = "application/json";
                res.ContentEncoding = Encoding.UTF8;

                var stringifiedStatus = Encoding.UTF8.GetBytes(statusManager.StatusJSON.ToString());

                res.ContentLength64 = stringifiedStatus.Length;
                res.Close(stringifiedStatus, false);

                return;
            }

            res.StatusCode = 404;
            res.Close();
        }
        public void Initialize()
        {
            server = new HttpServer(this.ServerPort);

            server.OnGet += (sender, e) =>
            {
                OnHTTPGet(e);
            };

            server.AddWebSocketService<StatusBroadcastBehavior>("/socket", initializer => initializer.SetStatusManager(this.statusManager));

            HttpSiraStatus.Plugin.Logger.Info("Starting HTTP server on port " + this.ServerPort);
            server.Start();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue) {
                if (disposing) {
                    // TODO: �}�l�[�W�h��Ԃ�j�����܂� (�}�l�[�W�h �I�u�W�F�N�g)
                    HttpSiraStatus.Plugin.Logger.Info("Stopping HTTP server");
                    server.Stop();
                }

                // TODO: �A���}�l�[�W�h ���\�[�X (�A���}�l�[�W�h �I�u�W�F�N�g) ��������A�t�@�C�i���C�U�[���I�[�o�[���C�h���܂�
                // TODO: �傫�ȃt�B�[���h�� null �ɐݒ肵�܂�
                disposedValue = true;
            }
        }

        // // TODO: 'Dispose(bool disposing)' �ɃA���}�l�[�W�h ���\�[�X���������R�[�h���܂܂��ꍇ�ɂ̂݁A�t�@�C�i���C�U�[���I�[�o�[���C�h���܂�
        // ~HTTPServer()
        // {
        //     // ���̃R�[�h��ύX���Ȃ��ł��������B�N���[���A�b�v �R�[�h�� 'Dispose(bool disposing)' ���\�b�h�ɋL�q���܂�
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // ���̃R�[�h��ύX���Ȃ��ł��������B�N���[���A�b�v �R�[�h�� 'Dispose(bool disposing)' ���\�b�h�ɋL�q���܂�
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
