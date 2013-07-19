using System;
using System.Reflection;
using System.Collections.Generic;

namespace SlapHappy {
	public class Reflector {
		/**
		 * Returns concrete subclasses of the given type within the assembly scope.
		 */
		public static List<Type> SubclassesOf(Type parentType) {
			List<Type> types = new List<Type>();

			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
				foreach (Type type in assembly.GetTypes())
					if (type.IsSubclassOf(parentType))
						types.Add(type);

			return types;
		}

		/**
		 * Returns instances for each subclass of the given type parameter.
		 */
		public static T[] InstantiateAll<T>() {
			List<Type> types = SubclassesOf(typeof(T));
			T[] instances = new T[types.Count];

			for (int i = 0; i < instances.Length; i++)
				instances[i] = (T) Activator.CreateInstance(types[i]);

			return instances;
		}
	}
}
