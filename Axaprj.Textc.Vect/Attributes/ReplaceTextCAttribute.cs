using Axaprj.Textc.Vect.Detectors;
using System;
using System.Collections.Generic;

namespace Axaprj.Textc.Vect.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public abstract class ReplaceTextCAttribute : ReplaceAttribute
    {
        List<string> _patternsList;
        ReplaceDetector _detector;

        public string SyntaxPattern
        {
            get => _patternsList[0];
            set => SyntaxPatterns = new string[] { value };
        }
        public string[] SyntaxPatterns
        {
            get => _patternsList.ToArray();
            set => _patternsList = new List<string>(value);
        }

        public abstract Delegate CreateFnResultTask();

        public ReplaceDetector Detector
        {
            get => _detector = _detector ?? new ReplaceDetector(this);
        }
    }
}
