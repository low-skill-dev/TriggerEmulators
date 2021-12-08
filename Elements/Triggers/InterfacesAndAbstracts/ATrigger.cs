﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emulators
{
    public abstract class ATrigger :ICurrentSource,ITrigger
    {
		// Складывается ложное ощущение, что это конкретно RSC триггер, но на деле JK Триггер просто
		// должен переименовать названия каналов, через свойства, в остальном их не надо переписывать.
		private ICurrentSource _S_ChannelInput;
		protected ICurrentSource S_ChannelInput
		{
			get => _S_ChannelInput;
			set
			{
				value.OutputChanged += OnAnyVoltageChange;
				_S_ChannelInput = value;
			}
		}
		private ICurrentSource _C_ChannelInput;
		protected ICurrentSource C_ChannelInput
		{
			get => _C_ChannelInput;
			set
			{
				value.OutputChanged += OnAnyVoltageChange;
				_C_ChannelInput = value;
			}
		}
		private ICurrentSource _R_ChannelInput;
		protected ICurrentSource R_ChannelInput
		{
			get => _R_ChannelInput;
			set
			{
				value.OutputChanged += OnAnyVoltageChange;
				_R_ChannelInput = value;
			}
		}
		protected abstract void OnAnyVoltageChange(object sender, EventArgs e);
		protected abstract void CalculateState();

		protected event EventHandler? StateChanged;
		public event EventHandler? OutputChanged
		{
			add => StateChanged += value;
			remove => StateChanged -= value;
		}
		protected event EventHandler? StatePendingChange;

		private TRIGGER_STATES _CurrentState;
		public TRIGGER_STATES CurrentState
		{
			get => _CurrentState;
			set
			{
				StatePendingChange?.Invoke(this, EventArgs.Empty);
				_CurrentState = value;
				StateChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		public CurrentHistory OutputHistory { get; protected set; }

		public float CurrentLevel_Volt
		{
			get => StateToVolt(this.CurrentState);
		}
		private float StateToVolt(TRIGGER_STATES state) => state switch
		{
			TRIGGER_STATES.Q_IsOne => 1f,
			TRIGGER_STATES.Q_IsZero => 0f,
			TRIGGER_STATES.Q_IsIncorrect => float.NaN,
			_ => throw new NotImplementedException()
		};

		public ATrigger()
		{
			
		}
	}
}
