import { Navbar } from "./_components/navbar";

const MainLayout = ({ children }: { children: React.ReactNode }) => {
  return (
    <div className="flex min-h-full flex-col">
      <Navbar />
      <main className="mx-auto w-full max-w-5xl flex-1 px-4 py-10">
        {children}
      </main>
      <footer className="text-muted-foreground mx-auto w-full max-w-5xl px-4 py-6 text-xs">
        Next.js {"·"} ASP.NET Core {"·"} Clerk {"—"} replace this
        shell with your own.
      </footer>
    </div>
  );
};

export default MainLayout;
