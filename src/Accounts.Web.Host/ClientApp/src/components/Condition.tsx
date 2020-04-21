export default ({ val, children }: any) => {
  if (val) {
    return children;
  }

  return null;
};
