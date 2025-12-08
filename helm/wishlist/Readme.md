# Wishlist App Helm Chart

This is a helm chart for the Wishlist App.

## Installation

- Create the target namespace and add the label for the trust-manager:

```bash
kubectl create namespace <NAMESPACE>
```

- Create the following secret in this namespace with the needed values before installing the chart:

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

- Create a ``values.yaml`` file with the following content:

```yaml
hostname: <The hostname of the app>
```

- Install or upgrade the chart and provide the ``values.yaml`` file created earlier:

```bash
helm install wishlist oci://ghcr.io/damidhagor/wishlist -n <NAMESPACE> --values=<VALUES>.yaml --version 1.6.0
```
