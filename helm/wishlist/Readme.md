# Wishlist App Helm Chart

This is a helm chart for the Wishlist App.

## Installation

- Create the target namespace and add the label for the trust-manager:

```bash
kubectl create namespace <NAMESPACE>
```

- Create the Harbor secret in the namespace:

```bash
kubectl create secret docker-registry harbor --docker-server=harbor.damidhagor.de --docker-username=<USERNAME> --docker-password=<PASSWORD> -n <NAMESPACE>
```

- Create the following secrets in this namespace with the needed values before installing the chart:

```yaml
apiVersion: v1
kind: Secret
metadata:
  name: wishlist-app
  namespace: <NAMESPACE>
type: Opaque
stringData:
  mongodbConnectionString: ""
  mongodbDatabaseName: ""
  oidcAuthority: ""
  oidcClientId: ""
  oidcClientSecret: ""
  oidcScopes: "[\"openid\", \"profile\", \"email\", \"offline_access\"]"
```

```yaml
apiVersion: v1
kind: Secret
metadata:
  name: aspire-dashboard
  namespace: <NAMESPACE>
type: Opaque
stringData:
  apikey: ""
```

- Create a ``values.yaml`` file with the following content:

```yaml
hostname: <The hostname of the app (required)>
app:
  tag: "1.1.5-preview"
aspire:
  tag: "9.3.0"
```

	Only the ``hostname`` value is required. The other values are optional.

- Install or upgrade the chart and provide the ``values.yaml`` file created earlier:

```bash
helm install <RELEASE_NAME> oci://harbor.damidhagor.de/wishlist/wishlist -n <NAMESPACE> --values=<VALUES>.yaml --version <VERSION>
```
