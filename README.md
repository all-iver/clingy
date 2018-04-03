Clingy
------

Clingy is a general purpose attachments/relationships library for Unity (https://unity3d.com).  An attachment could be something like a physics joint, an object following another object, a parent/child relationship, or your own custom definition.  Clingy helps you create and manage these types of attachments at runtime.  For instance, you could use Clingy to chain a group of objects together using physics joints, allow the player to pick up various weapons, position objects during base building, or just have objects follow each other around.  In particular, Unity's pattern of creating physics joint components in the editor is not very helpful when you want to create and destroy joints at runtime.  Clingy lets you configure the type of joint you want in the editor, but create the joint components themselves at runtime.

Clingy also lets you write code to transition objects in and out of attachments, and define attach points on objects to define values for anchors and rotations, etc.  

In the editor, you can use an `Attacher` component to very quickly play around with and test your `AttachStrategies`.

In code, Clingy works something like this:

```
using SubC.Attachments;
Attachment attachment = Clingy.AttachChain(myChainStrategy, gameObject1, gameObject2);
// get notified on the detached event
attachment.events.OnDetached.AddListener(eventInfo => { Debug.Log("Detached!"); });
// add a new object to the chain on the fly
attachment.objects.Add(new AttachObject(someGameObject, 1));
// time passes...
attachment.Detach();
```

An `Attachment` gives you a standard interface for doing things like `Attach()`, `Detach()`, adding/removing objects at runtime, and listening to events.

Installing
-----

To install, just copy the `Clingy` folder into your Unity `Assets` folder.

Attach Strategies
-----

An `AttachStrategy` does the actual work of connecting objects to each other.  Clingy has various strategies built in, or you can create custom ones by subclassing `AttachStrategy` (which is a subclass of `ScriptableObject`).

There are built-in strategies for Physics (2D and 3D), Following and Parenting.  Each type has One-to-One, Many-to-One and Chain versions.  There is also a built-in Symbolic strategy which does nothing, but still sends attachment-related events.

To use a built-in strategy, go to the `Assets->Create->Clingy` menu and configure it the way you want it.  Then either create an `Attacher` component in the editor or get a handle to the strategy in code and create a new `Attachment`:

```
using SubC.Attachments;
Attachment attachment = new Attachment(myStrategy, new AttachObject(gameObject1, 0), ...);
attachment.Attach();
// or use one of the Clingy.Attach...() convenience methods
```

Each object should be an `AttachObject` which specifies a `GameObject` and the category of the object within the `AttachStrategy` (i.e., the strategy might have one category for the Target object and another category for Follower objects).

Transitioners
-----

Often when attaching objects at runtime, an object will need to be prepared in some way.  For instance, you may want a physics object to have its `Rigidbody` component removed while it is attached, and then restored after it is detached.  With Clingy, you can add a `Transitioner` to your `AttachStrategy` for this purpose.  This allows you to add custom code to transition an object in and out of an `Attachment` without having to modify the strategy itself.  You can specify a `Transitioner` per category, meaning that all objects in an `AttachStrategy` that share the same category in the strategy will also use the same `Transitioner`.

Clingy comes with a built-in `FlexibleTransitioner` that is able to do a range of common useful things, but for anything complicated you'll want to create your own custom transition code by subclassing `Transitioner` (like `AttachStrategy`, `Transitioner` is a subclass of `ScriptableObject`).

A `Transitioner` is able to run code every frame while objects are attached, so if you want you can use it to do more than just transition an object in and out of an `Attachment`.  The difference between a `Transitioner` and an `AttachStrategy` is that a `Transitioner` only acts on one object, while an `AttachStrategy` acts on all objects in the `Attachment`.

Params
-----

When configuring an `AttachStrategy`, the strategy may need certain pieces of information like anchor positions, rotation amounts, speeds, distances, etc.  Sometimes it makes sense to hardcode these values directly into the strategy, but in other cases you might want to put data on objects directly, so that each object can specify its own anchor point, for example.

Clingy has a flexible system that lets you put these pieces of data, called `Params`, wherever makes the most sense for your application.  Each `Param` can be identified by a name you give it.  Then in the `AttachStrategy` configuration there's a dropdown for the `Provider` of the `Param`, where you can specify which object is responsible for providing that `Param`'s value.  Your options depend on the specific strategy, but for instance in a Physics strategy you could specify that the Jointed object is responsible for providing a `Param` called `'position'` to use as the joint's anchor.  The strategy will then look on the Jointed object for an `AttachPoint` component.  If it finds one, the strategy will ask the `AttachPoint` for a `Param` called `'position'`.  The value provided can then be interpreted as local to the Jointed or the Connected object (or as a world position), and finally used as the joint's anchor.  If no `Param` can be found, the strategy uses a default value you specify.

A `Param` is not limited to just positions and rotations - it can also be a `Color` or a `GameObject`, for instance.

Attach Points
-----

An `AttachPoint`'s job is to provide `Params` for objects in an `Attachment`.  For instance, the object may specify positions and rotations relative to itself that tell an `AttachStrategy` how to do its job.

An `AttachPoint` can also provide `Params` for other objects in an `Attachment` besides the one the `AttachPoint` component is actually on.  For instance, when you include an object with a `GridAttachPoint` component in an `Attachment`, all other objects in the `Attachment` will have `Params` applied to them that represents their positions snapped to the grid defined by the first object.  Then the `AttachStrategy` can be configured to move the attaching objects to their snapped positions.  The result is that any object that attaches to the `GridAttachPoint` will be snapped to a grid.

There are a few other simple built-in `AttachPoints` in Clingy, but you can easily create your own by subclassing `AttachPoint`.

Sprite Params
-----

An `AttachPoint` can use a different set of `Params` for each frame in a sprite animation.  To do so, go to `Assets->Create->Clingy->Sprite Params`.  Using this editor interface, you can define the `Params` you want to be applied for a given sprite.  That is to say, when an object's `SpriteRenderer` is using a certain sprite, the `AttachPoint` component on that object will return the `Params` you define.  Make sure you drag the `SpriteParams` to the `AttachPoint`.

As an example, by using this interface you could have a sprite hold a sword in a different position depending on which frame the sprite's animation is using.