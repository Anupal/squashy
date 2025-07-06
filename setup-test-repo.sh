REPO_NAME="test-repo"

echo "recreating directory '$REPO_NAME'"
rm -rf $REPO_NAME
mkdir $REPO_NAME

echo "setup git repo"
pushd $REPO_NAME
git init

echo "create commit sequence"
for i in {1..3}; do
    echo "this is file $i" > "file$i.txt"
    git add "file$i.txt"
    git -c user.name="Test user $i" -c user.email="test-user$i@test.com" commit -m "add file$i"
done

echo "modify commit sequence"
for i in {1..3}; do
    echo " modified" >> "file$i.txt"
    git add "file$i.txt"
    git -c user.name="Test user $i" -c user.email="test-user$i@test.com" commit -m "modify file$i"
done

echo "delete commit sequence"
for i in {1..3}; do
    rm "file$i.txt"
    git add "file$i.txt"
    git -c user.name="Test user $i" -c user.email="test-user$i@test.com" commit -m "remove file$i"
done