namespace SubC.Attachments {

    public enum ParamType {
        AngleLimits = 2,
        Bool = 5,
        Color = 10,
        Curve = 15,
        // Direction = 20,
        // FlipX = 25,
        // FlipY = 30,
        Float = 35,
        Gradient = 45,
        Integer = 50,
        Layer = 52,
        Object = 55,
        // Position = 60,
        Rotation = 65,
        // SortingOrder = 70,
        String = 75,
        Vector3 = 80
    }

    // public enum ParamBlendMode {
    //     None,
    //     Add,
    //     Multiply
    // }

    // public enum ParamRelativityType {
    //     None,
    //     Provider,
    //     ProviderOrWorld,
    //     BlendProvider
    // }

    public enum ParamRelativityType {
        None,
        Normal
    }

    public enum ParamNormalRelativity {
        Local, // this means relative to the provider that gives us the param value
        World,
        Object // the user can choose another object (from the providers list for now) 
    }

    public enum PositionTransformType {
        Full,
        None,
        RotationOnly
    }

}