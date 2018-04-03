namespace SubC.Attachments {

    using UnityEngine;

    public enum EaseType {
        BackIn,
        BackOut,
        BackInOut,
        BounceIn,
        BounceOut,
        BounceInOut,
        CircIn,
        CircOut,
        CircInOut,
        CubicIn,
        CubicOut,
        CubicInOut,
        ElasticIn,
        ElasticOut,
        ElasticInOut,
        ExpoIn,
        ExpoOut,
        ExpoInOut,
        Linear,
        QuadIn,
        QuadOut,
        QuadInOut,
        QuartIn,
        QuartOut,
        QuartInOut,
        QuintIn,
        QuintOut,
        QuintInOut,
        SineIn,
        SineOut,
        SineInOut
    }

    public static class Easing {
        // easing functions adapted from https://github.com/jesusgollonet/ofpennereasing.  thanks!

        /*
        TERMS OF USE - EASING EQUATIONS

        Open source under the BSD License. 

        Copyright © 2001 Robert Penner
        All rights reserved.

        Redistribution and use in source and binary forms, with or without modification, are permitted provided that 
        the following conditions are met:

        * Redistributions of source code must retain the above copyright notice, this list of conditions and the 
          following disclaimer.  
        * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the 
          following disclaimer in the documentation and/or other materials provided with the distribution.  
        * Neither the name of the author nor the names of contributors may be used to endorse or promote products 
          derived from this software without specific prior written permission.  
        
        THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED 
        WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A 
        PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY 
        DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, 
        PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER 
        CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR 
        OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH 
        DAMAGE.
        */

        public delegate float EasingFunction(float t, float b, float c, float d);

        public static EasingFunction FromEaseType(EaseType easeType) {
            switch (easeType) {
                case EaseType.Linear:
                    return Linear;
                case EaseType.BackIn:
                    return BackIn;
                case EaseType.BackOut:
                    return BackOut;
                case EaseType.BackInOut:
                    return BackInOut;
                case EaseType.BounceIn:
                    return BounceIn;
                case EaseType.BounceOut:
                    return BounceOut;
                case EaseType.BounceInOut:
                    return BounceInOut;
                case EaseType.CircIn:
                    return CircIn;
                case EaseType.CircOut:
                    return CircOut;
                case EaseType.CircInOut:
                    return CircInOut;
                case EaseType.CubicIn:
                    return CubicIn;
                case EaseType.CubicOut:
                    return CubicOut;
                case EaseType.CubicInOut:
                    return CubicInOut;
                case EaseType.ElasticIn:
                    return ElasticIn;
                case EaseType.ElasticOut:
                    return ElasticOut;
                case EaseType.ElasticInOut:
                    return ElasticInOut;
                case EaseType.ExpoIn:
                    return ExpoIn;
                case EaseType.ExpoOut:
                    return ExpoOut;
                case EaseType.ExpoInOut:
                    return ExpoInOut;
                case EaseType.QuadIn:
                    return QuadIn;
                case EaseType.QuadOut:
                    return QuadOut;
                case EaseType.QuadInOut:
                    return QuadInOut;
                case EaseType.QuartIn:
                    return QuartIn;
                case EaseType.QuartOut:
                    return QuartOut;
                case EaseType.QuartInOut:
                    return QuartInOut;
                case EaseType.QuintIn:
                    return QuintIn;
                case EaseType.QuintOut:
                    return QuintOut;
                case EaseType.QuintInOut:
                    return QuintInOut;
                case EaseType.SineIn:
                    return SineIn;
                case EaseType.SineOut:
                    return SineOut;
                case EaseType.SineInOut:
                    return SineInOut;
            }
            throw new System.NotImplementedException("Invalid ease type.");
        }

        public static float BackIn(float t, float b, float c, float d) {
            float s = 1.70158f;
            float postFix = t/=d;
            return c*(postFix)*t*((s+1)*t - s) + b;
        }

        public static float BackOut(float t, float b, float c, float d) {	
            float s = 1.70158f;
            return c*((t=t/d-1)*t*((s+1)*t + s) + 1) + b;
        }

        public static float BackInOut(float t, float b, float c, float d) {
            float s = 1.70158f;
            if ((t/=d/2) < 1) return c/2*(t*t*(((s*=(1.525f))+1)*t - s)) + b;
            float postFix = t-=2;
            return c/2*((postFix)*t*(((s*=(1.525f))+1)*t + s) + 2) + b;
        }

        public static float BounceIn(float t, float b, float c, float d) {
            return c - BackOut (d-t, 0, c, d) + b;
        }

        public static float BounceOut(float t, float b, float c, float d) {
            if ((t/=d) < (1/2.75f)) {
                return c*(7.5625f*t*t) + b;
            } else if (t < (2/2.75f)) {
                float postFix = t-=(1.5f/2.75f);
                return c*(7.5625f*(postFix)*t + .75f) + b;
            } else if (t < (2.5/2.75)) {
                    float postFix = t-=(2.25f/2.75f);
                return c*(7.5625f*(postFix)*t + .9375f) + b;
            } else {
                float postFix = t-=(2.625f/2.75f);
                return c*(7.5625f*(postFix)*t + .984375f) + b;
            }
        }

        public static float BounceInOut(float t, float b, float c, float d) {
            if (t < d/2) return BounceIn (t*2, 0, c, d) * .5f + b;
            else return BounceOut (t*2-d, 0, c, d) * .5f + c*.5f + b;
        }

        public static float CircIn(float t, float b, float c, float d) {
            return -c * (Mathf.Sqrt(1 - (t/=d)*t) - 1) + b;
        }

        public static float CircOut(float t, float b, float c, float d) {
            return c * Mathf.Sqrt(1 - (t=t/d-1)*t) + b;
        }

        public static float CircInOut(float t, float b, float c, float d) {
            if ((t/=d/2) < 1) return -c/2 * (Mathf.Sqrt(1 - t*t) - 1) + b;
            return c/2 * (Mathf.Sqrt(1 - t*(t-=2)) + 1) + b;
        }

        public static float CubicIn(float t, float b, float c, float d) {
            return c*(t/=d)*t*t + b;
        }

        public static float CubicOut(float t, float b, float c, float d) {
            return c*((t=t/d-1)*t*t + 1) + b;
        }

        public static float CubicInOut(float t, float b, float c, float d) {
            if ((t/=d/2) < 1) return c/2*t*t*t + b;
            return c/2*((t-=2)*t*t + 2) + b;	
        }

        public static float ElasticIn(float t, float b, float c, float d) {
            if (t==0) return b;  if ((t/=d)==1) return b+c;  
            float p=d*.3f;
            float a=c; 
            float s=p/4;
            float postFix =a*Mathf.Pow(2,10*(t-=1)); // this is a fix, again, with post-increment operators
            return -(postFix * Mathf.Sin((t*d-s)*(2*Mathf.PI)/p )) + b;
        }

        public static float ElasticOut(float t, float b, float c, float d) {
            if (t==0) return b;  if ((t/=d)==1) return b+c;  
            float p=d*.3f;
            float a=c; 
            float s=p/4;
            return (a*Mathf.Pow(2,-10*t) * Mathf.Sin( (t*d-s)*(2*Mathf.PI)/p ) + c + b);	
        }

        public static float ElasticInOut(float t, float b, float c, float d) {
            if (t==0) return b;  if ((t/=d/2)==2) return b+c; 
            float p=d*(.3f*1.5f);
            float a=c; 
            float s=p/4;
            float postFix;
            if (t < 1) {
                postFix =a*Mathf.Pow(2,10*(t-=1)); // postIncrement is evil
                return -.5f*(postFix* Mathf.Sin( (t*d-s)*(2*Mathf.PI)/p )) + b;
            } 
            postFix =  a*Mathf.Pow(2,-10*(t-=1)); // postIncrement is evil
            return postFix * Mathf.Sin( (t*d-s)*(2*Mathf.PI)/p )*.5f + c + b;
        }

        public static float ExpoIn(float t, float b, float c, float d) {
            return (t==0) ? b : c * Mathf.Pow(2, 10 * (t/d - 1)) + b;
        }

        public static float ExpoOut(float t, float b, float c, float d) {
            return (t==d) ? b+c : c * (-Mathf.Pow(2, -10 * t/d) + 1) + b;	
        }

        public static float ExpoInOut(float t, float b, float c, float d) {
            if (t==0) return b;
            if (t==d) return b+c;
            if ((t/=d/2) < 1) return c/2 * Mathf.Pow(2, 10 * (t - 1)) + b;
            return c/2 * (-Mathf.Pow(2, -10 * --t) + 2) + b;
        }

        public static float Linear(float t, float b, float c, float d) {
            return c*t/d + b;
        }

        public static float QuadIn(float t, float b, float c, float d) {
            return c*(t/=d)*t + b;
        }

        public static float QuadOut(float t, float b, float c, float d) {
            return -c *(t/=d)*(t-2) + b;
        }

        public static float QuadInOut(float t, float b, float c, float d) {
            if ((t/=d/2) < 1) return ((c/2)*(t*t)) + b;
            return -c/2 * (((t-2)*(--t)) - 1) + b;
            /*
            originally return -c/2 * (((t-2)*(--t)) - 1) + b;
            
            I've had to swap (--t)*(t-2) due to diffence in behaviour in 
            pre-increment operators between java and c++, after hours
            of joy
            */
        }

        public static float QuartIn(float t, float b, float c, float d) {
            return c*(t/=d)*t*t*t + b;
        }

        public static float QuartOut(float t, float b, float c, float d) {
            return -c * ((t=t/d-1)*t*t*t - 1) + b;
        }

        public static float QuartInOut(float t, float b, float c, float d) {
            if ((t/=d/2) < 1) return c/2*t*t*t*t + b;
            return -c/2 * ((t-=2)*t*t*t - 2) + b;
        }

        public static float QuintIn(float t, float b, float c, float d) {
            return c*(t/=d)*t*t*t*t + b;
        }

        public static float QuintOut(float t, float b, float c, float d) {
            return c*((t=t/d-1)*t*t*t*t + 1) + b;
        }

        public static float QuintInOut(float t, float b, float c, float d) {
            if ((t/=d/2) < 1) return c/2*t*t*t*t*t + b;
            return c/2*((t-=2)*t*t*t*t + 2) + b;
        }

        public static float SineIn(float t, float b, float c, float d) {
            return -c * Mathf.Cos(t/d * (Mathf.PI/2)) + c + b;
        }

        public static float SineOut(float t, float b, float c, float d) {	
            return c * Mathf.Sin(t/d * (Mathf.PI/2)) + b;	
        }

        public static float SineInOut(float t, float b, float c, float d) {
            return -c/2 * (Mathf.Cos(Mathf.PI*t/d) - 1) + b;
        }

    }

}