#!/bin/bash


cd ./PUC.TechTree
dotnet publish
cd ./bin/Debug/net5.0/publish
docker build -t reg.myvar.global/puc .
docker push reg.myvar.global/puc

cd ../../../../../
cd Kubernetes
kubectl delete -f Deployment.yaml
kubectl delete -f Ingress.yaml
kubectl delete -f Service.yaml
kubectl apply -f Deployment.yaml
kubectl apply -f Service.yaml
kubectl apply -f Ingress.yaml
cd ..