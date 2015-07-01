using System;

namespace VisualCrypt.Cryptography.Portable.APIV2.DataTypes
{
	public class Response
	{
		const string Success = "SUCCESS";
		const string Cancelled = "CANCELLED";

		string _state;

		public Response() { }

		public Response(string errorMessage)
		{
			_state = errorMessage;
		}

		public bool IsSuccess
		{
			get
			{
				CheckStateHasBeenSet();
				return _state == Success;
			}
		}

		public bool IsCancelled
		{
			get
			{
				CheckStateHasBeenSet();
				return _state == Cancelled;
			}
		}

		public string Error
		{
			get
			{
				CheckStateHasBeenSet();

				if(_state == Cancelled || _state == Success)
					throw new InvalidOperationException(string.Format("State of Response is '{0}'. Please check IsSuccess and IsCancelled before accessing Respose.Error unnecessarily.",_state));

				return _state;
			}
		}
		

		public void SetSuccess()
		{
			CheckNotSettingStateTwice();
			_state = Success;
		}

		public void SetError(Exception e)
		{
			CheckNotSettingStateTwice();

			if (e is OperationCanceledException)
				_state = Cancelled;
			else
			{
				_state = e.Message;
			}
		}

		public void SetError(string errorMessage)
		{
			CheckNotSettingStateTwice();

			if (string.IsNullOrWhiteSpace(errorMessage))
				throw new ArgumentNullException("errorMessage");

			_state = errorMessage;
		}

		void CheckNotSettingStateTwice()
		{
			if (_state != null)
				throw new InvalidOperationException("The state of the response must not be set more than one time.");
		}

		void CheckStateHasBeenSet()
		{
			if (_state == null)
				throw new InvalidOperationException("The state of the response has not been set.");
		}
	}

	public sealed class Response<T> : Response
	{
		public Response() { }
		public Response(string errorMessage) : base(errorMessage) { }

		public T Result;



	}

	public sealed class Response<T, T2> : Response
	{
		public Response() { }
		public Response(string errorMessage) : base(errorMessage) { }
		public T Result;
		public T2 Result2;
	}


}