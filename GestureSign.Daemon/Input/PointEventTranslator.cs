﻿using System;
using System.Collections.Generic;
using System.Linq;
using GestureSign.Common.Configuration;
using GestureSign.Common.Input;
using ManagedWinapi.Hooks;

namespace GestureSign.Daemon.Input
{
    public class PointEventTranslator
    {
        private int _lastPointsCount;
        private HashSet<MouseActions> _pressedMouseButton;

        internal PointEventTranslator(InputProvider inputProvider)
        {
            _pressedMouseButton = new HashSet<MouseActions>();
            inputProvider.TouchInputProcessor.PointsIntercepted += TranslateTouchEvent;
            inputProvider.LowLevelMouseHook.MouseDown += LowLevelMouseHook_MouseDown;
            inputProvider.LowLevelMouseHook.MouseMove += LowLevelMouseHook_MouseMove;
            inputProvider.LowLevelMouseHook.MouseUp += LowLevelMouseHook_MouseUp;
        }

        #region Custom Events

        public event EventHandler<InputPointsEventArgs> PointDown;

        protected virtual void OnPointDown(InputPointsEventArgs args)
        {
            if (PointDown != null) PointDown(this, args);
        }

        public event EventHandler<InputPointsEventArgs> PointUp;

        protected virtual void OnPointUp(InputPointsEventArgs args)
        {
            if (PointUp != null) PointUp(this, args);
        }

        public event EventHandler<InputPointsEventArgs> PointMove;

        protected virtual void OnPointMove(InputPointsEventArgs args)
        {
            if (PointMove != null) PointMove(this, args);
        }

        #endregion

        #region Private Methods

        private void LowLevelMouseHook_MouseUp(LowLevelMouseMessage mouseMessage, ref bool handled)
        {
            if ((MouseActions)mouseMessage.Button == AppConfig.DrawingButton)
            {
                var args = new InputPointsEventArgs(new List<InputPoint>(new[] { new InputPoint(1, mouseMessage.Point) }), Device.Mouse);
                OnPointUp(args);
                handled = args.Handled;
            }
            _pressedMouseButton.Remove((MouseActions)mouseMessage.Button);
        }

        private void LowLevelMouseHook_MouseMove(LowLevelMouseMessage mouseMessage, ref bool handled)
        {
            var args = new InputPointsEventArgs(new List<InputPoint>(new[] { new InputPoint(1, mouseMessage.Point) }), Device.Mouse);
            OnPointMove(args);
        }

        private void LowLevelMouseHook_MouseDown(LowLevelMouseMessage mouseMessage, ref bool handled)
        {
            if ((MouseActions)mouseMessage.Button == AppConfig.DrawingButton && _pressedMouseButton.Count == 0)
            {
                var args = new InputPointsEventArgs(new List<InputPoint>(new[] { new InputPoint(1, mouseMessage.Point) }), Device.Mouse);
                OnPointDown(args);
                handled = args.Handled;
            }
            _pressedMouseButton.Add((MouseActions)mouseMessage.Button);
        }

        private void TranslateTouchEvent(object sender, RawPointsDataMessageEventArgs e)
        {
            int releaseCount = e.RawData.Count(rtd => !rtd.Tip);
            if (releaseCount != 0)
            {
                if (e.RawData.Count <= _lastPointsCount)
                {
                    OnPointUp(new InputPointsEventArgs(e.RawData, Device.Touch));
                    _lastPointsCount = _lastPointsCount - releaseCount;
                }
                return;
            }

            if (e.RawData.Count > _lastPointsCount)
            {
                if (PointCapture.Instance.InputPoints.Any(p => p.Count > 10))
                {
                    OnPointMove(new InputPointsEventArgs(e.RawData, Device.Touch));
                    return;
                }
                _lastPointsCount = e.RawData.Count;
                OnPointDown(new InputPointsEventArgs(e.RawData, Device.Touch));
            }
            else if (e.RawData.Count == _lastPointsCount)
            {
                OnPointMove(new InputPointsEventArgs(e.RawData, Device.Touch));
            }
        }

        #endregion
    }
}
