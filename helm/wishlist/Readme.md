# Wishlist App Helm Chart

This is a helm chart for the Wishlist App.

## Installation

- Create the target namespace and add the label for the trust-manager:

	```bash
	kubectl create namespace <NAMESPACE>
    kubectl label namespace <NAMESPACE> injectSelfSignedCACertificate=true
	```

- Create the following secrets in this namespace with the needed values before installing the chart:

	```yaml
	apiVersion: v1
	kind: Secret
	metadata:
	  name: app-secret
	  namespace: <NAMESPACE>
	type: Opaque
	data:
	  identity-authority:
	  identity-client-id:
	  identity-client-secret:
	  identity-scopes:
	```

	```yaml
	apiVersion: v1
	kind: Secret
	metadata:
	  name: mongodb-user-admin-password
	  namespace: <NAMESPACE>
	type: Opaque
	stringData:
      password:
	```

	```yaml
	apiVersion: v1
	kind: Secret
	metadata:
	  name: mongodb-user-wishlist-app-password
	  namespace: <NAMESPACE>
	type: Opaque
	stringData:
      password:
	```

- Create a ``values.yaml`` file with the following content:

	```yaml
	hostname: <The hostname of the app (required)>
	postgres:
	  size: <Size of the Postgres DB (default 5Gi)>
	app:
	  image: <The app's image (default harbor.damidhagor.de/wishlist/wishlist-app)>
	  tag: <The app's image tag (default latest)>
	```

	Only the ``hostname`` value is required. The other values are optional.

- Install or upgrade the chart and provide the ``values.yaml`` file created earlier:

	```bash
	helm install <RELEASE_NAME> <CHART_LOCATION> -n <NAMESPACE> --create-namespace --values=values.yaml
    helm install <RELEASE_NAME> oci://harbor.damidhagor.de/wishlist/wishlist --version <TAG> -n <NAMESPACE> --values=<VALUES>.yaml
	```
