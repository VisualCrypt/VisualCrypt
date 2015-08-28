using System;
using System.Threading;

namespace VisualCrypt.Cryptography.VisualCrypt2.Infrastructure
{
    public sealed class LongRunningOperation : IDisposable
    {
        readonly CancellationTokenSource _cancellationTokenSource;
        readonly LongRunningOperationContext _context;
        readonly Action _switchbackToPreviousBar;

        public LongRunningOperation(Action<EncryptionProgress> reportAction, Action switchbackToPreviousBar)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _switchbackToPreviousBar = switchbackToPreviousBar;
            _context = new LongRunningOperationContext(_cancellationTokenSource.Token, new EncryptionProgress(reportAction));
        }

        public LongRunningOperationContext Context
        {
            get { return _context; }
        }


        public void Cancel()
        {

            if (!_isDispsed && _cancellationTokenSource.Token.CanBeCanceled)
                _cancellationTokenSource.Cancel();
            _switchbackToPreviousBar();
        }

        bool _isDispsed;
        public void Dispose()
        {
            if (!_isDispsed)
                _cancellationTokenSource.Dispose();
            _isDispsed = true;
        }
    }
}