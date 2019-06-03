
BASEDIR=$(dirname "$0")

ROOT=$BASEDIR/../

cd $ROOT

FULL_ROOT=`pwd`

OUT=$FULL_ROOT"/_output"
echo "output dir: "$OUT

mkdir -p $OUT

#It will use <PropertyGroup> and  <ItemGroup>(referenced pack or item with property pack=true)
dotnet pack src/WeGouSharp/WeGouSharp.csproj -o $OUT -c Release | tee

